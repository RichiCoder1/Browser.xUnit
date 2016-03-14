#load "tools/Utilities.cake"
#addin "Cake.Json"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var rootDir = Context.Environment.WorkingDirectory;
var artifactsDir = rootDir + Directory("/artifacts");
var srcDir = rootDir + Directory("/src");
var toolsDir = rootDir + Directory("/tools");

var versionInfo = GitVersion();
var isAppVeyorBuild = AppVeyor.IsRunningOnAppVeyor;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    ResilientCleanDirs(new DirectoryPath[] { artifactsDir });
});

Task("Update-Version")
    .Does(() => 
{
    var projectFiles = GetFiles("./src/**/project.json");
    foreach (var jsonFile in projectFiles) {
        var projectContents = System.IO.File.ReadAllText(jsonFile.FullPath);
        var json = ParseJson(projectContents);
        json["version"] = versionInfo.FullSemVer;
        System.IO.File.WriteAllText(jsonFile.FullPath, json.ToString());
    }
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var exitCode = StartProcess("dotnet", new ProcessSettings { Arguments = "restore" });
    if(exitCode != 0)
    {
        throw new CakeException("Failed to restore packages.");
    }
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{   
    var projectDir = srcDir + Directory("/Browser.xUnit");
    var exitCode = StartProcess("dotnet", new ProcessSettings()
        .WithArguments(args => 
        {
            args.Append("build");
            args.AppendQuoted(projectDir);
            args.Append("--configuration {0}", configuration);
        }));
        
    if (exitCode != 0)
    {
        throw new CakeException("Failed to build project");
    }
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
{   
    var projectDir = srcDir + Directory("/Browser.xUnit");
    var projectArtifactsDir = artifactsDir + Directory("/bin/Browser.xUnit");
    var exitCode = StartProcess("dotnet", new ProcessSettings()
        .WithArguments(args => 
        {
            args.Append("pack");
            args.AppendQuoted(projectDir);
            args.Append("--configuration {0}", configuration);
            args.Append("--output {0}", projectArtifactsDir);
        }));
        
    if (exitCode != 0)
    {
        throw new CakeException("Failed to pack project");
    }
    
    if (isAppVeyorBuild) {
        
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

