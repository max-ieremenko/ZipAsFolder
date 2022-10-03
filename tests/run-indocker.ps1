#Requires -Version "7.0"
#Requires -Modules @{ ModuleName="Pester"; ModuleVersion="5.3.3" }

param (
    [Parameter()]
    [string[]]
    $Script,

    [Parameter()]
    [string]
    $Image = "mcr.microsoft.com/powershell:7.0.0-ubuntu-18.04"
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Get-Command Invoke-Pester | Out-Null
$pester = Get-Module Pester
$pesterPath = Split-Path $pester.Path -Parent

$binPath = Join-Path $PSScriptRoot "../bin/module"
$binPath = [System.IO.Path]::GetFullPath($binPath)

docker run `
    -it --rm `
    -v "${binPath}:/root/.local/share/powershell/Modules/ZipAsFolder" `
    -v "${pesterPath}:/root/.local/share/powershell/Modules/Pester" `
    -v "${PSScriptRoot}:/app" `
    --entrypoint pwsh `
    --name "ZipAsFolder" `
    $Image `
    "/app/scripts/Run-TestsDocker.ps1" `
    -Output Detailed `
    $Script
