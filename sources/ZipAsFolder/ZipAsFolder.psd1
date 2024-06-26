@{
	RootModule = "ZipAsFolder"

	ModuleVersion = "3.2.1"
	GUID = "{7337A9C2-990A-4DEA-AB3A-9FFD4433C0DA}"

    CompanyName = "Max Ieremenko"
	Author = "Max Ieremenko"
	Copyright = "(C) 2022-2024 Max Ieremenko, licensed under MIT License"

	Description = @'
ZipAsFolder is a PowerShell FileSystem provider lets you get, add, change, clear, and delete files and directories in zip archives.

Example
    New-Item zf:/archive.zip # new empty zip archive
    Set-Content zf:/archive.zip/file.txt "hello from zf" # add a text file into the archive
    New-Item zf:/archive.zip/directory -ItemType Directory # add a directory into the archive
    Get-Content zf:/archive.zip/file.txt -Raw | Set-Content zf:/archive.zip/directory/file.txt
    Get-ChildItem zf:/archive.zip # list items of the archive
    New-Item zf:/extracted -ItemType Directory # new empty directory
    Copy-Item zf:/archive.zip/* zf:/extracted -Recurse # expand the archive
    Remove-Item zf:/archive.zip/directory -Recurse # remove a directory from the archive
    Remove-Item zf:/archive.zip -Recurse # remove the archive
    New-Item zf:/archive.zip
    Copy-Item zf:/extracted/* zf:/archive.zip -Recurse # compress
'@

	PowerShellVersion = "7.0"
	CompatiblePSEditions = @("Core")
	ProcessorArchitecture = "None"

	FormatsToProcess  = @("ZipAsFolder.Format.ps1xml")

	PrivateData = @{
		PSData = @{
			Tags = "zip", "archive", "extract", "compress", "PSEdition_Core", "Windows", "Linux", "macOS"
			LicenseUri = "https://build.todo"
			ProjectUri = "https://build.todo"
			ReleaseNotes = "https://build.todo"
		}
	 }
}