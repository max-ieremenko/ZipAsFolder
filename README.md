# ZipAsFolder

[![PowerShell Gallery](https://img.shields.io/powershellgallery/v/ZipAsFolder.svg?style=flat-square)](https://www.powershellgallery.com/packages/ZipAsFolder)

ZipAsFolder is a PowerShell FileSystem provider lets you get, add, change, clear, and delete files and directories in zip archives.

```powershell
PS> Install-Module -Name ZipAsFolder
```

After installation new `zf` PSDrive is available. On Windows the Root is empty, on Linux the Root is `/`.

```powershell
PS> Get-PSDrive

Name Provider     Root
---- --------     ----
zf   ZipAsFolder  /
```

## Usage

Usage is simple: just add `zf:` at the beginning of the path, like this "c:\my\path" => "zf:c:\my\path" on windows or "/my/path" => "zf:/my/path" on linux.

```powershell
# create a new empty zip archive file
PS> New-Item zf:/archive.zip

# add a text file into the archive
PS> Set-Content zf:/archive.zip/file.txt "hello from ZipAsFolder"

# add an empty directory into the archive
PS> New-Item zf:/archive.zip/directory -ItemType Directory

# create another text file
PS> Get-Content zf:/archive.zip/file.txt -Raw | Set-Content zf:/archive.zip/directory/file.txt

# show content
PS> Get-ChildItem zf:/archive.zip #-Recurse

# create an empty directory
PS> New-Item zf:/extracted -ItemType Directory

# expand the archive
PS> Copy-Item zf:/archive.zip/* zf:/extracted -Recurse

# remove "directory" from the archive
PS> Remove-Item zf:/archive.zip/directory -Recurse

# remove the archive
PS> Remove-Item zf:/archive.zip -Recurse

# compress the content of extracted
PS> New-Item zf:/archive.zip
PS> Copy-Item zf:/extracted/* zf:/archive.zip -Recurse
```

> ### Note
> on widows relative path works well
> ```powershell
> PS> cd c:\my\path
> PS> Get-ChildItem zf:.\ # the content of c:\my\path
> ```
> on linux the path must be fully qualified
> ```powershell
> PS> cd /my/path
> PS> Get-ChildItem zf:./ # the content of the root /
> PS> Get-ChildItem zf:/my/path # the content of /my/path
> ```

## Customization

show the configuration

```powershell
PS> $ZipAsFolder

ZipExtensions  CompressionLevel
-------------  ----------------
{.zip, .nupkg}          Optimal
```

ZipAsFolder recognizes a file as an archive by its extension, by default `.zip` and `.nupkg`.

To add `.docx` into the list, just change the configuration

```powershell
PS> $ZipAsFolder.ZipExtensions += ".docx"
```

## Motivation

I wrote and maintain the module just for fun and for my own needs. If it's useful for you too, that's great.

The module version `0.*` means the beta/stabilization phase. If you find a bug or misbehavior, please do me a favor by reporting it.