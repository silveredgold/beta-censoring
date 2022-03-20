#addin nuget:?package=Cake.Docker&version=1.1.0
#addin nuget:?package=Cake.Npm&version=2.0.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var runtimeSpec = Argument<string>("publish-runtimes", "win-x64;osx-x64;linux-x64");
var singleFile = Argument<bool>("single-file", true);

///////////////////////////////////////////////////////////////////////////////
// VERSIONING
///////////////////////////////////////////////////////////////////////////////

var packageVersion = string.Empty;
#load "build/version.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/BetaCensor.sln");
var solution = ParseSolution(solutionPath);
var projects = GetProjects(solutionPath, configuration);
var artifacts = "./dist/";
var runtimes = runtimeSpec.Split(';').ToList();

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	packageVersion = BuildVersion(fallbackVersion);
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in projects.AllProjectPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin/" + configuration);
		CleanDirectories(path + "/**/obj/" + configuration);
	}
	Information("Cleaning common files...");
	CleanDirectory(artifacts);
});

Task("Restore")
	.Does(() =>
{
	// Restore all NuGet packages.
	Information("Restoring solution...");
	foreach (var project in projects.AllProjectPaths) {
		DotNetRestore(project.FullPath);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	var settings = new DotNetBuildSettings {
		Configuration = configuration,
		NoIncremental = true,
		ArgumentCustomization = args => args
			.Append($"/p:Version={packageVersion}")
			.Append("/p:AssemblyVersion=1.0.0.0")
	};
	DotNetBuild(solutionPath, settings);
});


Task("NuGet")
	.IsDependentOn("Build")
	.Does(() =>
{
	Information("Building NuGet package");
	CreateDirectory(artifacts + "package/");
	var packSettings = new DotNetPackSettings {
		Configuration = configuration,
		NoBuild = true,
		OutputDirectory = $"{artifacts}package",
		ArgumentCustomization = args => args
			.Append($"/p:Version=\"{packageVersion}\"")
			.Append("/p:NoWarn=\"NU1701 NU1602\"")
	};
	foreach(var project in projects.SourceProjectPaths) {
		Information($"Packing {project.GetDirectoryName()}...");
		DotNetPack(project.FullPath, packSettings);
	}
});

Task("Pages-Build")
	.Does(() =>
{
	Information("Building Status page site");
	var siteRootPath = "./src/BetaCensor.Web.Status/ClientApp";
	// if (!string.IsNullOrWhiteSpace(EnvironmentVariable("GITHUB_REF"))) {
	NpmCi(settings => { settings.FromPath(siteRootPath); });
	// } else {
	// 	NpmInstall(settings => { settings.FromPath(siteRootPath); });
	// }
	NpmRunScript("build", settings => settings.FromPath(siteRootPath));
});

Task("Publish-NuGet-Package")
.IsDependentOn("NuGet")
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("NUGET_TOKEN")))
.WithCriteria(() => HasEnvironmentVariable("GITHUB_REF"))
.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v") || EnvironmentVariable("GITHUB_REF") == "refs/heads/main")
.Does(() => {
    var nugetToken = EnvironmentVariable("NUGET_TOKEN");
    var pkgFiles = GetFiles($"{artifacts}package/*.nupkg");
	Information($"Pushing {pkgFiles.Count()} package files!");
    NuGetPush(pkgFiles, new NuGetPushSettings {
      Source = "https://api.nuget.org/v3/index.json",
      ApiKey = nugetToken
    });
});

Task("Publish-Runtime")
	.IsDependentOn("Build")
	.IsDependentOn("Pages-Build")
	.Does(() =>
{
	var projectDir = $"{artifacts}server";
	var projPath = "./src/BetaCensor.Server/BetaCensor.Server.csproj";
	CreateDirectory(projectDir);
	DotNetPublish(projPath, new DotNetPublishSettings {
		OutputDirectory = projectDir + "/dotnet-any",
		Configuration = configuration,
		PublishSingleFile = false,
		PublishTrimmed = false,
		ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}").Append("/p:AssemblyVersion=1.0.0.0")
	});
	foreach (var runtime in runtimes) {
		var runtimeDir = $"{projectDir}/{runtime}";
		CreateDirectory(runtimeDir);
		Information("Publishing for {0} runtime", runtime);
		var settings = new DotNetPublishSettings {
			Runtime = runtime,
			SelfContained = true,
			Configuration = configuration,
			OutputDirectory = runtimeDir,
			PublishSingleFile = singleFile,
			PublishTrimmed = true,
			IncludeNativeLibrariesForSelfExtract = singleFile,
			ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}").Append("/p:AssemblyVersion=1.0.0.0")
		};
		DotNetPublish(projPath, settings);
        if (singleFile) {
            CleanDirectory(runtimeDir, fsi => fsi.Path.FullPath.EndsWith("onnxruntime_providers_shared.lib") || fsi.Path.FullPath.EndsWith("onnxruntime_providers_shared.pdb") || fsi.Path.FullPath.EndsWith("web.config"));
        }
		CleanDirectory(runtimeDir, fsi => fsi.Path.FullPath.EndsWith("onnxruntime.pdb") || fsi.Path.FullPath.EndsWith("onnxruntime.lib"));
		CreateDirectory($"{artifacts}archive");
		Zip(runtimeDir, $"{artifacts}archive/betacensor-server-{runtime}.zip");
	}
});

Task("Build-Docker-Image")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("Publish-Runtime")
	.Does(() =>
{
    // also shamelessly stolen from GitHub
    var dockerFileName = "Dockerfile";
	Information("Building Docker image...");
	CopyFileToDirectory($"./build/{dockerFileName}", artifacts);
	var bSettings = new DockerImageBuildSettings {
        Tag = new[] { $"beta-censoring/server:{packageVersion}", $"quay.io/beta-censoring/server:{packageVersion}"},
        File = artifacts + dockerFileName,
        BuildArg = new[] {$"package_version={packageVersion}"}
    };
	DockerBuild(bSettings, artifacts);
	DeleteFile(artifacts + dockerFileName);
});

Task("Publish-Docker-Image")
.IsDependentOn("Build-Docker-Image")
.WithCriteria(IsRunningOnUnix())
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("QUAY_TOKEN")))
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("QUAY_USER")))
.Does(() => {
    DockerLogin(new DockerRegistryLoginSettings{
        Password = EnvironmentVariable("QUAY_TOKEN"),
        Username = EnvironmentVariable("QUAY_USER")
    }, "quay.io");
    DockerPush($"quay.io/beta-censoring/server:{packageVersion}");
});

Task("Default")
	.IsDependentOn("Build");

Task("Publish")
	.IsDependentOn("NuGet")
	.IsDependentOn("Publish-Runtime")
    .IsDependentOn("Build-Docker-Image");

Task("Release")
	.IsDependentOn("Publish")
	.IsDependentOn("Publish-NuGet-Package")
    .IsDependentOn("Publish-Docker-Image");

RunTarget(target);



public class ProjectCollection {
	public IEnumerable<SolutionProject> SourceProjects {get;set;}
	public IEnumerable<DirectoryPath> SourceProjectPaths {get { return SourceProjects.Select(p => p.Path.GetDirectory()); } } 
	public IEnumerable<SolutionProject> TestProjects {get;set;}
	public IEnumerable<DirectoryPath> TestProjectPaths { get { return TestProjects.Select(p => p.Path.GetDirectory()); } }
	public IEnumerable<SolutionProject> AllProjects { get { return SourceProjects.Concat(TestProjects); } }
	public IEnumerable<DirectoryPath> AllProjectPaths { get { return AllProjects.Select(p => p.Path.GetDirectory()); } }
}

ProjectCollection GetProjects(FilePath slnPath, string configuration) {
	var solution = ParseSolution(slnPath);
	var projects = solution.Projects.Where(p => p.Type != "{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
	var testAssemblies = projects.Where(p => p.Name.Contains(".Tests")).Select(p => p.Path.GetDirectory() + "/bin/" + configuration + "/" + p.Name + ".dll");
	return new ProjectCollection {
		SourceProjects = projects.Where(p => !p.Name.Contains(".Tests")),
		TestProjects = projects.Where(p => p.Name.Contains(".Tests"))
	};
}