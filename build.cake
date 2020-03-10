#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"
#addin "Cake.FileHelpers&version=3.2.1"
#addin "Cake.Incubator&version=5.1.0"

using System.Text.RegularExpressions;

var configuration = Argument("configuration", "Release");
var target = Argument("target", "Default");

var project = File("./SlackAPI/SlackApi.csproj");
var testProject = File("./SlackAPI.Tests/SlackApi.Tests.csproj");
var testConfig = File("./SlackAPI.Tests/Configuration/config.json");
var projects = new[] { project, testProject };
var artifactsDirectory = "./artifacts";
GitVersion gitVersion = null;
var isReleaseBuild = false;

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDirectory);
});


Task("Configure")
    .Does(() =>
{
    gitVersion = GitVersion();

    GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true,
        UpdateAssemblyInfoFilePath = "GlobalAssemblyInfo.cs"
    });

    isReleaseBuild = AppVeyor.IsRunningOnAppVeyor
        ? AppVeyor.Environment.Repository.Branch == "master"
        : false;

    Information("Is release build: '{0}'", isReleaseBuild);
    Information("GitVersion details:\n{0}", gitVersion.Dump());

    if (AppVeyor.IsRunningOnAppVeyor)
    {
        var buildVersion = gitVersion.SemVer + ".ci." + AppVeyor.Environment.Build.Number;
        Information("Using build version: {0}", buildVersion);
        AppVeyor.UpdateBuildVersion(buildVersion);
    }
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
                Configuration = configuration
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
            IncludeSymbols = !isReleaseBuild,
            IncludeSource = !isReleaseBuild,
            ArgumentCustomization = args => args.Append("/p:Version=\"" + gitVersion.NuGetVersion + "\"")
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
