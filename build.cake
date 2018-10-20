#addin "Cake.FileHelpers"

var configuration = Argument("configuration", "Release");
var target = Argument("target", "Default");

var mainProject = File("./SlackAPI/SlackApi.csproj");
var testProject = File("./SlackAPI.Tests/SlackApi.Tests.csproj");
var testConfig = File("./SlackAPI.Tests/Configuration/config.json");
var projects = new[] { mainProject, testProject };
var artifactsDirectory = Directory("./artifacts");
var versionSuffix = "local0";
var isReleaseBuild = false;

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDirectory);
});


Task("Configure")
    .Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor)
    {
        isReleaseBuild = AppVeyor.Environment.Repository.Branch == "master"
                         && AppVeyor.Environment.Repository.Tag.IsTag;

        // If the build is a tag on master, generate a clean version (1.0.0) following SemVer 1.0.0 rules 
        // In other cases, append metadata to identify the prerelease (1.0.0-dev123shaabcdefg)
        versionSuffix = $"";
        if (!isReleaseBuild)
        {
          versionSuffix += $"dev{AppVeyor.Environment.Build.Number}sha{AppVeyor.Environment.Repository.Commit.Id.Substring(0, 8)}";
        }
    }

    var versionPrefix = XmlPeek("./Directory.Build.props", "/Project/PropertyGroup/VersionPrefix");
    var version = string.Join("-", versionPrefix, versionSuffix);

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        // Update AppVeyor build version so it will match the build version in assemblies and package
        AppVeyor.UpdateBuildVersion(version);
    }

    Information("Using version '{0}'", version);
    Information("Release type build (skip symbols): {0}", isReleaseBuild);
});


Task("Build")
    .IsDependentOn("Configure")
    .Does(() =>
{
    foreach(var project in projects)
    {
        DotNetCoreBuild(
            project,
            new DotNetCoreBuildSettings
            {
                Configuration = configuration,
                VersionSuffix = versionSuffix
            }
        );
    }
});

Task("ConfigureTest")
    .Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor)
    {
        FileWriteText(testConfig, $@"
        {{
            ""slack"":
            {{
                ""userAuthToken"": ""{EnvironmentVariable("userAuthToken")}"",
                ""botAuthToken"": ""{EnvironmentVariable("botAuthToken")}"",
                ""testChannel"": ""{EnvironmentVariable("testChannel")}"",
                ""directMessageUser"": ""{EnvironmentVariable("directMessageUser")}"",
                ""authCode"": ""{EnvironmentVariable("authCode")}"",
                ""clientId"": ""{EnvironmentVariable("clientId")}"",
                ""clientSecret"": ""{EnvironmentVariable("clientSecret")}""
            }}
        }}");
    }
});


Task("Test")
    .IsDependentOn("Configure")
    .IsDependentOn("ConfigureTest")
    .Does(() =>
{
    // AppVeyor is unable to differentiate tests from multiple frameworks
    // To push all test results on AppVeyor:
    // - disable builtin AppVeyor push from XUnit
    // - generate MSTest report
    // - replace assembly name in test report
    // - manualy push test result

    foreach (var framework in new[] { "net452", "netcoreapp2.1"})
    {
        DotNetCoreTest(
            testProject,
            new DotNetCoreTestSettings
            {
                Configuration = configuration,
                Framework = framework,
                ArgumentCustomization = args => args.Append("--logger \"trx;LogFileName=result_" + framework + ".trx\""),
                EnvironmentVariables = new Dictionary<string, string>{
                    { "APPVEYOR_API_URL", null }
                }
            }
        );

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            var testResult = File("./SlackAPI.Tests/TestResults/result_" + framework + ".trx");

            ReplaceRegexInFiles(
                testResult,
                @"slackapi\.tests\.dll",
                "SlackAPI.Tests." + framework + ".dll",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

           AppVeyor.UploadTestResults(testResult, AppVeyorTestResultsType.MSTest);
        }
    }
});


Task("Pack")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePack(
        mainProject,
        new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = artifactsDirectory,
            VersionSuffix = versionSuffix,
            IncludeSymbols = !isReleaseBuild,
            IncludeSource = !isReleaseBuild
        }
    );

});


Task("Default")
    .IsDependentOn("Pack");


RunTarget(target);
