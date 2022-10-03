#Requires -Version "7.0"
#Requires -Modules @{ ModuleName="Pester"; ModuleVersion="5.3.3" }

param (
    [Parameter()]
    [string[]]$Script
)

$pwsh = [System.Diagnostics.Process]::GetCurrentProcess().MainModule.FileName
$file = Join-Path $PSScriptRoot "scripts/Run-TestsLocally.ps1"

$binPath = Join-Path $PSScriptRoot "../bin/module"
$binPath = [System.IO.Path]::GetFullPath($binPath)

$argumentList = "-NoExit", "-File", $file, "-ModulePath", $binPath
$argumentList += $Script

Start-Process -FilePath $pwsh -ArgumentList $argumentList