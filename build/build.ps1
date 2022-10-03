#Requires -Version "7.0"
#Requires -Modules @{ ModuleName="InvokeBuild"; ModuleVersion="5.9.12"}
#Requires -Modules @{ ModuleName="Pester"; ModuleVersion="5.3.3" }

[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet("LocalBuild", "CIBuild")] 
    [string]
    $Task = "LocalBuild"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$file = Join-Path $PSScriptRoot "scripts/build-tasks.ps1"
Invoke-Build -File $file -Task $Task