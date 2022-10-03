param (
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $ModulePath,

    [Parameter(ValueFromRemainingArguments)]
    [string[]]
    $Script
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = Join-Path $PSScriptRoot ".."
$root = [System.IO.Path]::GetFullPath($root)

. (Join-Path $root "scripts/New-TestData.ps1")

Import-Module (Join-Path $ModulePath "ZipAsFolder.psd1")

$config = New-PesterConfiguration
$config.Output.Verbosity = "Detailed"
$config.Run.Exit = $true
$config.Run.Throw = $true

if ($Script) {
    $config.Run.Path = $Script | ForEach-Object { Join-Path $root $_ }
} else {
    $config.Run.Path = $root
}

Invoke-Pester -Configuration $config