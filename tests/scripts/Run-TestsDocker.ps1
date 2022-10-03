param (
    [Parameter(Mandatory)]
    [string]
    $Output,

    [Parameter(ValueFromRemainingArguments)]
    [string[]]
    $Script
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

Import-Module Pester
Import-Module ZipAsFolder

$root = Join-Path $PSScriptRoot ".."
$root = [System.IO.Path]::GetFullPath($root)

. (Join-Path $root "scripts/New-TestData.ps1")

$config = New-PesterConfiguration
$config.Output.Verbosity = $Output
$config.Run.Exit = $true
$config.Run.Throw = $true

if ($Script) {
    $config.Run.Path = $Script | ForEach-Object { Join-Path $root $_ }
}

Invoke-Pester -Configuration $config