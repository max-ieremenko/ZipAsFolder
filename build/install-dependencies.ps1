#Requires -Version "7.0"

[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet(".net", "InvokeBuild", "Pester")] 
    [string[]]
    $List
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-ModuleVersion {
    param (
        [Parameter(Mandatory)]
        [string]
        $Name
    )
    
    $sources = Get-Content (Join-Path $PSScriptRoot "build.ps1") -Raw
    $tokens = $null
    $errors = $null
    $modules = [Management.Automation.Language.Parser]::ParseInput($sources, [ref]$tokens, [ref]$errors).ScriptRequirements.RequiredModules
    foreach ($module in $modules) {
        if ($module.Name -eq $Name) {
            return $module.Version
        }
    }

    throw "Module $Name no found."
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not $List -or (".net" -in $List)) {
    $version = (Get-Content -Raw (Join-Path $PSScriptRoot "../sources/global.json") | ConvertFrom-Json).sdk.version
    $script = Join-Path $PSScriptRoot "scripts/step-install-dotnet.ps1"
    & $script $version
}

if (-not $List -or ("InvokeBuild" -in $List)) {
    $script = Join-Path $PSScriptRoot "scripts/step-install-module.ps1"
    $version = Get-ModuleVersion "InvokeBuild"
    & $script "InvokeBuild" $version
}

if (-not $List -or ("Pester" -in $List)) {
    $script = Join-Path $PSScriptRoot "scripts/step-install-module.ps1"
    $version = Get-ModuleVersion "Pester"
    & $script "Pester" $version
}