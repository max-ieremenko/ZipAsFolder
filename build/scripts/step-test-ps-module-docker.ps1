[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $TestsPath,

    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $ModulePath,

    [Parameter(Mandatory)]
    [string]
    $ImageName
)

task Test {
    Get-Command Invoke-Pester | Out-Null
    $pester = Get-Module Pester
    $pesterPath = Split-Path $pester.Path -Parent

    $entryPoint = Join-Path $PSScriptRoot "step-test-ps-module-docker-entrypoint.ps1"
    Exec {
        docker run `
            -it --rm `
            -v "${ModulePath}:/module.zip" `
            -v "${pesterPath}:/root/.local/share/powershell/Modules/Pester" `
            -v "${TestsPath}:/app" `
            -v "${entryPoint}:/entrypoint.ps1" `
            --entrypoint pwsh `
            $ImageName `
            "/entrypoint.ps1"
    }
}
