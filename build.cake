#addin nuget:?package=Cake.Git
#addin "MagicChunks"

var configuration = Argument("configuration", "Release");
var target = Argument("target", "Default");

var mainProject = File("./SlackAPI/SlackApi.Netcore.csproj");
var testProject = File("./SlackAPI.Tests/SlackApi.Tests.NetCore.csproj");
var projects = new[] { mainProject, testProject };
var artifactsDirectory = Directory("./artifacts");
var revision = AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number : 0;
var version = AppVeyor.IsRunningOnAppVeyor ? new Version(AppVeyor.Environment.Build.Version.Split('-')[0]).ToString(3) : "1.0.0";
var globalAssemblyInfo = File("./GlobalAssemblyVersion.cs");

var generatedVersion = "";
var generatedSuffix = "";


Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDirectory);
});


Task("Restore-Packages")
    .Does(() =>
{
    foreach(var project in projects)
    {
        DotNetCoreRestore(project);
    }
});


Task("Generate-Versionning")
    .Does(() =>
{
    generatedVersion = version + "." + revision;
    Information("Generated version '{0}'", generatedVersion);

    var branch = (AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Repository.Branch : GitBranchCurrent(".").FriendlyName).Replace('/', '-');
    generatedSuffix = (branch == "master" && revision > 0) ? "" : branch.Substring(0, Math.Min(10, branch.Length)) + "-" + revision;
    Information("Generated suffix '{0}'", generatedSuffix);
});


Task("Patch-GlobalAssemblyVersions")
    .IsDependentOn("Generate-Versionning")
    .Does(() =>
{
    CreateAssemblyInfo(globalAssemblyInfo, new AssemblyInfoSettings {
        FileVersion = generatedVersion,
        InformationalVersion = version + "-" + generatedSuffix,
        Version = generatedVersion
        }
    );
});


Task("Patch-ProjectJson")
    .IsDependentOn("Generate-Versionning")
    .Does(() =>
{
    TransformConfig(
        mainProject,
        mainProject,
        new TransformationCollection
        {
            { "Project/PropertyGroup/VersionPrefix", version },
            { "Project/PropertyGroup/VersionSuffix", generatedSuffix }
        }
    );
});


Task("Patch")
    .IsDependentOn("Patch-GlobalAssemblyVersions")
    .IsDependentOn("Patch-ProjectJson");


Task("Build")
    .IsDependentOn("Restore-Packages")
    .IsDependentOn("Patch")
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


Task("Test")
    .IsDependentOn("Restore-Packages")
    .IsDependentOn("Patch")
    .Does(() =>
{
    DotNetCoreTest(
        testProject,
        new DotNetCoreTestSettings
        {
            Configuration = configuration,
        }
    );
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
            VersionSuffix = generatedSuffix,
            ArgumentCustomization = args => args.Append("--include-symbols")
        }
    );

});


Task("Default")
    .IsDependentOn("Pack");


RunTarget(target);
