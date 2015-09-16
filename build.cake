#load "tools/Utilities.cake"
#line 2

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

// Define tool paths
var dnxDir = toolsDir + Directory("/dnx-clr-win-x86/bin");
var dnuTool = dnxDir + File("/dnu.cmd");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    ResilientCleanDirs(new DirectoryPath[] { artifactsDir });
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var exitCode = StartProcess(dnuTool, new ProcessSettings { Arguments = "restore --parallel --quiet" });
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
    var projectArtifactsDir = artifactsDir + Directory("/bin/Browser.xUnit");
    StartProcess(dnuTool, new ProcessSettings()
        .WithArguments(args => 
        {
            args.Append("pack");
            args.Append("--quiet");
            args.AppendQuoted(projectDir);
            args.Append("--configuration {0}", configuration);
            args.Append("--out {0}", projectArtifactsDir);
        }));
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

