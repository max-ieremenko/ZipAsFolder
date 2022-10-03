#Requires -Version "7.0"

$ErrorActionPreference = "Stop"

function Get-ShortVersion {
    [CmdletBinding()]
    param (
        [Parameter(ValueFromPipeline)]
        [string]
        $FullVersion
    )
    
    process {
        $parts = $FullVersion -split "-"
        $result = $parts[0]

        if ($parts[1] -like "preview*") {
            $result += "-" + $parts[1]
        }

        return $result
    }
}

(Invoke-RestMethod -Uri "https://mcr.microsoft.com/v2/powershell/tags/list").tags `
    | Where-Object {$_ -Like "[0-9]*"} `
    | Get-ShortVersion `
    | Sort-Object -Unique