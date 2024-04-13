[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $BinPath,

    [Parameter(Mandatory)]
    [ValidateSet("net8.0")] 
    [string]
    $Framework
)

$sourceDir = Join-Path $BinPath "unit-test/$Framework"
$testList = Get-ChildItem -Path $sourceDir -Filter *.Test.dll ` | ForEach-Object { $_.FullName }

assert ($testList -and $testList.Length) "$Framework test list is empty"

$testList
Exec { dotnet vstest $testList }
