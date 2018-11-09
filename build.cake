#tool "nuget:?package=GitVersion.CommandLine"
#addin "Cake.FileHelpers"

using System.Text.RegularExpressions;

var configuration = Argument("configuration", "Release");
var target = Argument("target", "Default");

var project = File("./SlackAPI/SlackApi.csproj");
var testProject = File("./SlackAPI.Tests/SlackApi.Tests.csproj");
var testConfig = File("./SlackAPI.Tests/Configuration/config.json");
var projects = new[] { project, testProject };
var artifactsDirectory = "./artifacts";
var versionSuffix = string.Empty;
var isReleaseBuild = false;

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDirectory);
});


Task("Configure")
    .Does(() =>
{
    var buildNumber = 0;
    if (AppVeyor.IsRunningOnAppVeyor)
    {
        isReleaseBuild = AppVeyor.Environment.Repository.Branch == "master" && AppVeyor.Environment.Repository.Tag.IsTag;
        buildNumber = AppVeyor.Environment.Build.Number;
        Information("Build number is '{0}' (CI build)", buildNumber);
    }
    else
    {
        buildNumber = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        Information("Build number is '{0}' (local build)", buildNumber);
    }

    // If the build is a tag on master, generate a clean version (1.0.0)
    // following SemVer 1.0.0 rules. NuGet supports only SemVer 1.0.0
    // In other cases, generate a prerelease version (1.0.0-branch.123+sha.abcdefg)
    // following SevVer 2.0.0 rules. MyGet supports SemVer 2.0.0
    if (isReleaseBuild)
    {
        versionSuffix = "\"\"";
    }
    else
    {
        var gitVersion = GitVersion();
        var gitBranch = (AppVeyor.IsRunningOnAppVeyor
            ? AppVeyor.Environment.Repository.Branch
            : gitVersion.BranchName);
        gitBranch = Regex.Replace(gitBranch, @"[/\-_]", string.Empty);
        gitBranch = gitBranch.Substring(0, Math.Min(10, gitBranch.Length));

        Information("Current git branch is '{0}' (normalized)", gitBranch);

        var gitCommitId = (AppVeyor.IsRunningOnAppVeyor
            ? AppVeyor.Environment.Repository.Commit.Id
            : gitVersion.Sha)
            .Substring(0, 8);

        Information("Current git sha is '{0}' (normalized)", gitCommitId);

        var isPullRequest = AppVeyor.IsRunningOnAppVeyor && AppVeyor.Environment.PullRequest.IsPullRequest;
        if (isPullRequest)
        {
            gitBranch = "PR";
        }

        Information("Is Pull Request: '{0}'", isPullRequest);


        versionSuffix = $"{gitBranch}.{buildNumber}+sha.{gitCommitId}";
    }

    var versionPrefix = XmlPeek("./Directory.Build.props", "/Project/PropertyGroup/VersionPrefix");
    var version = isReleaseBuild ? $"{versionPrefix}-release.{buildNumber}" : string.Join("-", versionPrefix, versionSuffix);
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
                ""clientId"": ""{EnvironmentVariable("clientId")}"",
                ""clientSecret"": ""{EnvironmentVariable("clientSecret")}"",
                ""redirectUrl"": ""{EnvironmentVariable("redirectUrl")}"",
                ""authUsername"": ""{EnvironmentVariable("authUsername")}"",
                ""authPassword"": ""{EnvironmentVariable("authPassword")}"",
                ""authWorkspace"": ""{EnvironmentVariable("authWorkspace")}""
            }}
        }}");
    }
});


Task("Test")
    .IsDependentOn("ConfigureTest")
    .IsDependentOn("Build")
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
                RegexOptions.IgnoreCase);

           AppVeyor.UploadTestResults(testResult, AppVeyorTestResultsType.MSTest);
        }
    }
});


Task("Package")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePack(
        project,
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


Task("Publish")
    .IsDependentOn("Package")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor && !AppVeyor.Environment.PullRequest.IsPullRequest, "Publishing is supported only from CI for non PR")
    .Does(() =>
{
    // Publish on Nuget if it's a release build or on MyGet for others builds
    var mapping = new Dictionary<bool, (string token, string provider, string source)>
    {
        { true, ("NUGET_APITOKEN", "NuGet", "https://nuget.org/api/v2/package") },
        { false, ("MYGET_APITOKEN", "MyGet", "https://www.myget.org/F/slackapi/api/v2") },
    };

    var config = mapping[isReleaseBuild];

    var apiToken = EnvironmentVariable(config.token);
    if (string.IsNullOrEmpty(apiToken))
    {
        Warning("{0} environment variable not found. Unable to push package on {1}", config.token, config.provider);
    }
    else
    {
        var packages = GetFiles(artifactsDirectory + "/**/*.nupkg");

        NuGetPush(packages, new NuGetPushSettings
        {
            Source = config.source,
            ApiKey = apiToken,
            Verbosity = NuGetVerbosity.Detailed,
        });
    }
});


Task("Default")
    .IsDependentOn("Package")
    .IsDependentOn("Publish");


RunTarget(target);
