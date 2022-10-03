[CmdletBinding()]
param (
    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path $_ })]
    [string]
    $BinPath,

    [Parameter(Mandatory)]
    [ValidateScript({ Test-Path (Split-Path $_ -Parent) })]
    [string]
    $OutPath,

    [Parameter(Mandatory)]
    [string]
    $ModuleVersion,

    [Parameter(Mandatory)]
    [string]
    $RepositoryUrl
)

$temp = Join-Path (Split-Path $OutPath -Parent) "tmp"
remove $temp
remove $OutPath

Copy-Item -Path $BinPath -Destination $temp -Recurse

$psdFile = Get-Item (Join-Path $temp "*.psd1")
assert ($psdFile) ".psd1 not found"

Update-ModuleManifest `
    -Path $psdFile.FullName `
    -ModuleVersion $ModuleVersion `
    -LicenseUri "$RepositoryUrl/blob/main/LICENSE" `
    -ProjectUri $RepositoryUrl `
    -ReleaseNotes "$RepositoryUrl/releases"

Get-ChildItem $temp -Include *.pdb -Recurse | Remove-Item

# remove comments and empty lines
$draftContent = Get-Content $psdFile
$cleanContent = @()
$descriptionBegin = $false
for ($i = 0; $i -lt $draftContent.Count; $i++) {
    $line = $draftContent[$i]
    $test = $line.Trim()

    if ($test.StartsWith("Description =")) {
        $descriptionBegin = $true
    }
    elseif ($test.StartsWith("# Minimum version") -or $test.StartsWith("PowerShellVersion =")) {
        $descriptionBegin = $false
    }

    if (-not $descriptionBegin) {
        if ([string]::IsNullOrWhiteSpace($test) -or $test.StartsWith("#")) {
            continue
        }
    
        $index = $line.LastIndexOf(" #")
        if ($index -gt 0) {
            $line = $line.Substring(0, $index)
        }
    }

    $cleanContent += $line
}

Set-Content $psdFile $cleanContent

Compress-Archive -Path (Join-Path $temp "*") -DestinationPath $OutPath

remove $temp
