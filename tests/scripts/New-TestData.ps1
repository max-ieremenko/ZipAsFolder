function New-TestData {
    param (
        [Parameter(Mandatory)]
        [string]
        $Path
    )

    Remove-Item "$Path/*" -Recurse
    
    $source = Join-Path $PSScriptRoot "../test-data/*"
    Copy-Item -Path $source -Destination $Path -Recurse
}