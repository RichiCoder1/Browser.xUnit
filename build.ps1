Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Debug",
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Verbose",
    [switch]$Experimental,
    [switch]$WhatIf
)

$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"
$NUGET_URL = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$CAKE_EXE = Join-Path $TOOLS_DIR "Cake/Cake.exe"
$DOTNET_DIR = Join-Path $TOOLS_DIR "dotnet"

# Should we use experimental build of Roslyn?
$UseExperimental = "";
if($Experimental.IsPresent) {
    $UseExperimental = "-experimental"
}

# Is this a dry run?
$UseDryRun = "";
if($WhatIf.IsPresent) {
    $UseDryRun = "-dryrun"
}

# Try download NuGet.exe if do not exist.
if (!(Test-Path $NUGET_EXE)) {
    (New-Object System.Net.WebClient).DownloadFile($NUGET_URL, $NUGET_EXE)
}

# Make sure NuGet exists where we expect it.
if (!(Test-Path $NUGET_EXE)) {
    Throw "Could not find NuGet.exe"
}

# Restore tools from NuGet?
if(-Not $SkipToolPackageRestore.IsPresent)
{
    Push-Location
    Set-Location $TOOLS_DIR
    Invoke-Expression "$NUGET_EXE install -ExcludeVersion"
    Pop-Location
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
	}
}

# Make sure that Cake has been installed.
if (!(Test-Path $CAKE_EXE)) {
    Throw "Could not find Cake.exe"
}

# Make sure dotnet is installed
if((Get-Command "dotnet" -ErrorAction SilentlyContinue) -eq $null)
{
	Invoke-WebRequest -Uri https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/install.ps1 -OutFile install.ps1
	./install.ps1 -version 1.0.0.001718 -channel beta
	$InstallDir = "$env:LocalAppData\Microsoft\dotnet"
	if (!(Test-Path $InstallDir)) 
	{
		throw "Failed to install dotnet"
	}
	$env:Path += ";$env:LocalAppData\Microsoft\dotnet\cli\bin"
}

# Start Cake
Invoke-Expression "$CAKE_EXE `"$Script`" -target=`"$Target`" -configuration=`"$Configuration`" -verbosity=`"$Verbosity`" $UseDryRun $UseExperimental"
Write-Host
exit $LASTEXITCODE
