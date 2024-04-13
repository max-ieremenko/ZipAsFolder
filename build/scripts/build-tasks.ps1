task LocalBuild DefaultBuild, LocalPsCoreTest
task CIBuild DefaultBuild, CIPsCoreTest

task DefaultBuild Initialize, Clean, Build, PackPowerShellModule, UnitTest

Enter-Build {
    $settings = @{
        sources       = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../../sources"))
        bin           = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../../bin"))
        tests         = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../../tests"))
        version       = $(
            $buildProps = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../../sources/Directory.Build.props"))
            $packageVersion = (Select-Xml -Path $buildProps -XPath "Project/PropertyGroup/Version").Node.InnerText
            assert $packageVersion "Version not found"
            $packageVersion
        )
        repositoryUrl = $(
            $url = Exec { git config --get remote.origin.url }
            if ($url.EndsWith(".git")) {
                $url = $url.Substring(0, $url.Length - 4)
            }

            $url
        )
    }
}

task Initialize {
    $env:GITHUB_SHA = Exec { git rev-parse HEAD }
}

task Clean {
    remove $settings.bin

    Get-ChildItem -Path $settings.sources -Filter bin -Directory -Recurse | Remove-Item -Recurse -Force
    Get-ChildItem -Path $settings.sources -Filter obj -Directory -Recurse | Remove-Item -Recurse -Force
}

task Build {
    $solutionFile = Join-Path $settings.sources "ZipAsFolder.sln"

    Exec { dotnet restore $solutionFile }
    
    Exec { 
        dotnet build `
            -t:Rebuild `
            -p:Configuration=Release `
            $solutionFile 
    }
}

task UnitTest {
    Exec { .\step-unit-test.ps1 $settings.bin "net8.0" }
}

task PackPowerShellModule {
    Exec { 
        .\step-pack-ps-module.ps1 `
            -BinPath (Join-Path $settings.bin "module") `
            -OutPath (Join-Path $settings.bin "pwsh-module.zip") `
            -ModuleVersion $settings.version `
            -RepositoryUrl $settings.repositoryUrl
    }
}

task LocalPsCoreTest {
    # show-powershell-images.ps1
    $images = $(
        "mcr.microsoft.com/powershell:7.0.0-ubuntu-18.04"
        , "mcr.microsoft.com/powershell:7.0.1-ubuntu-18.04"
        , "mcr.microsoft.com/powershell:7.0.2-ubuntu-18.04"
        , "mcr.microsoft.com/powershell:7.0.3-ubuntu-18.04"
        , "mcr.microsoft.com/powershell:7.1.0-ubuntu-18.04"
        , "mcr.microsoft.com/powershell:7.1.1-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.1.2-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.1.3-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.1.4-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.1.5-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.2.0-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.2.1-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.2.2-ubuntu-20.04"
        , "mcr.microsoft.com/powershell:7.3-ubuntu-20.04")

    $builds = @()
    foreach ($image in $images) {
        exec { docker pull --quiet $image }

        $builds += @{
            File       = "step-test-ps-module-docker.ps1"
            Task       = "Test"
            TestsPath  = $settings.tests
            ModulePath = (Join-Path $settings.bin "pwsh-module.zip")
            ImageName  = $image
        }
    }

    Build-Parallel $builds -ShowParameter ImageName -MaximumBuilds 4
}

task CIPsCoreTest {
    Exec { 
        .\step-test-ps-module.ps1 `
            -TestsPath $settings.tests `
            -ModulePath (Join-Path $settings.bin "pwsh-module.zip")
    }
}