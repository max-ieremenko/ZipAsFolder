[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $TestsPath,

    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $ModulePath
)

$tempDir = Join-Path ([System.IO.Path]::GetTempPath()) ([guid]::NewGuid().ToString())
Expand-Archive $ModulePath $tempDir

$script = Join-Path $TestsPath "scripts/Run-TestsLocally.ps1"
& $script -ModulePath $tempDir
