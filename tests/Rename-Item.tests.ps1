BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Rename-Item file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Rename-Item file-1.txt new-file-1.txt" {
        $actual = Rename-Item "$location/file-1.txt" new-file-1.txt -PassThru

        $actual.Attributes | Should -Be "File"
        $actual.Name | Should -Be "new-file-1.txt"
        Get-Content new-file-1.txt | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Rename-Item file-1.txt file-2.txt -Force" {
        { Rename-Item "$location/file-1.txt" file-2.txt -Force } | Should -Throw
    }

    It "Rename-Item file-1.txt dir-1/file-2.txt -Force" {
        { Rename-Item "$location/file-1.txt" dir-1/file-2.txt -Force } | Should -Throw
    }
}

Describe "Rename-Item directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Rename-Item dir-1 new-dir-1" {
        $actual = Rename-Item "$location/dir-1" new-dir-1 -PassThru

        $actual.Attributes | Should -Be "Directory"
        Get-Content new-dir-1/sub-dir/file1-sub-dir.txt | Should -Be @("line")

        Test-Path dir-1 | Should -Be $false
    }

    It "Rename-Item dir-1 file-2.txt" {
        { Rename-Item "$location/dir-1" file-2.txt } | Should -Throw
    }

    It "Rename-Item dir-1 dir-1/dir-1" {
        { Rename-Item "$location/dir-1" dir-1/dir-1 } | Should -Throw
    }
}

Describe "Rename-Item zip file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Rename-Item Content.zip new-content.zip" {
        $actual = Rename-Item "$location/content.zip" new-content.zip -PassThru

        $actual.Attributes | Should -Be "File, Archive"
        Get-Content "$location/new-content.zip/11.txt" | Should -Be @("11")

        Test-Path Content.zip | Should -Be $false
    }

    It "Rename-Item Content.zip/inner.zip new-inner.zip" {
        $actual = Rename-Item "$location/Content.zip/inner.zip" new-inner.zip -PassThru

        $actual.Attributes | Should -Be "File, Archive"
        Get-Content "$location/Content.zip/new-inner.zip/11.txt" | Should -Be @("11")

        Test-Path "$location/Content.zip/inner.zip" | Should -Be $false
    }

    It "Rename-Item Content.zip/2 new-2" {
        $actual = Rename-Item "$location/Content.zip/2" new-2 -PassThru

        $actual.Attributes | Should -Be "Directory"
        Get-Content "$location/Content.zip/new-2/2.2/2.2.txt" | Should -Be @()

        Test-Path "$location/Content.zip/2" | Should -Be $false
    }

    It "Rename-Item Content.zip/11.txt new-11.txt" {
        $actual = Rename-Item "$location/Content.zip/11.txt" new-11.txt -PassThru

        $actual.Attributes | Should -Be "File"
        Get-Content "$location/Content.zip/new-11.txt" | Should -Be @("11")

        Test-Path "$location/Content.zip/11.txt" | Should -Be $false
    }

    It "Rename-Item Content.zip/inner.zip/2 new-2" {
        $actual = Rename-Item "$location/Content.zip/inner.zip/2" new-2 -PassThru

        $actual.Attributes | Should -Be "Directory"
        Get-Content "$location/Content.zip/inner.zip/new-2/22.txt" | Should -Be @("22")

        Test-Path "$location/Content.zip/inner.zip/2" | Should -Be $false
    }

    It "Rename-Item Content.zip/inner.zip/11.txt new-11.txt" {
        $actual = Rename-Item "$location/Content.zip/inner.zip/11.txt" new-11.txt -PassThru

        $actual.Attributes | Should -Be "File"
        Get-Content "$location/Content.zip/inner.zip/new-11.txt" | Should -Be @("11")

        Test-Path "$location/Content.zip/inner.zip/11.txt" | Should -Be $false
    }
}