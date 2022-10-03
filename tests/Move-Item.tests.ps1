BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Move-Item file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Move-Item file-1.txt new-file-1.txt" {
        $actual = Move-Item "$location/file-1.txt" "$location/new-file-1.txt" -PassThru

        $actual.Attributes | Should -Be "File"
        $actual.Name | Should -Be "new-file-1.txt"
        Get-Content new-file-1.txt | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Move-Item file-1.txt file-2.txt" {
        { Move-Item "$location/file-1.txt" "$location/file-2.txt" } | Should -Throw
    }

    It "Move-Item file-1.txt file-2.txt -Force" {
        $actual = Move-Item "$location/file-1.txt" "$location/file-2.txt" -Force -PassThru

        $actual.Attributes | Should -Be "File"
        $actual.Name | Should -Be "file-2.txt"
        Get-Content file-2.txt | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Move-Item file-1.txt dir-1" {
        $actual = Move-Item "$location/file-1.txt" "$location/dir-1" -PassThru

        $actual.Name | Should -Be "file-1.txt"
        Get-Content dir-1/file-1.txt | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Move-Item file-1.txt dir-1/file1-dir1.txt -Force" {
        $actual = Move-Item "$location/file-1.txt" "$location/dir-1/file1-dir1.txt" -Force -PassThru

        $actual.Name | Should -Be "file1-dir1.txt"
        Get-Content dir-1/file1-dir1.txt | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Move-Item file-1.txt new-dir/file-1.txt -Force" {
        { Move-Item "$location/file-1.txt" "$location/new-dir/file-1.txt" -Force } | Should -Throw
    }
}

Describe "Move-Item directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Move-Item dir-1 new-dir-1" {
        Move-Item "$location/dir-1" "$location/new-dir-1"

        $actual = Get-Item "$location/new-dir-1"
        $actual.Attributes | Should -Be "Directory"
        $actual.Name | Should -Be "new-dir-1"

        Test-Path dir-1 | Should -Be $false

        (Get-Item "$location/new-dir-1/file1-dir1.txt").Attributes | Should -Be "File"
        (Get-Item "$location/new-dir-1/sub-dir").Attributes | Should -Be "Directory"
        (Get-Item "$location/new-dir-1/sub-dir/file1-sub-dir.txt").Attributes | Should -Be "File"
    }

    It "Move-Item dir-1 existing" {
        New-Item existing -ItemType Directory | Out-Null
        Move-Item "$location/dir-1" "$location/existing"

        $actual = Get-Item "$location/existing/dir-1"
        $actual.Attributes | Should -Be "Directory"
        $actual.Name | Should -Be "dir-1"

        Test-Path dir-1 | Should -Be $false

        (Get-Item "$location/existing/dir-1/sub-dir/file1-sub-dir.txt").Attributes | Should -Be "File"
    }
}

Describe "Move-Item zip file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Move-Item file-1.txt content.zip/new-file-1.txt" {
        $actual = Move-Item "$location/file-1.txt" "$location/content.zip/new-file-1.txt" -PassThru

        $actual.Name | Should -Be "new-file-1.txt"
        Get-Content "$location/content.zip/new-file-1.txt" | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Move-Item file-1.txt content.zip/11.txt" {
        { Move-Item "$location/file-1.txt" "$location/content.zip/11.txt" } | Should -Throw
    }

    It "Move-Item file-1.txt content.zip/11.txt -Force" {
        $actual = Move-Item "$location/file-1.txt" "$location/content.zip/11.txt" -Force -PassThru

        $actual.Name | Should -Be "11.txt"
        Get-Content "$location/content.zip/11.txt" | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }

    It "Move-Item file-1.txt content.zip" {
        $actual = Move-Item "$location/file-1.txt" "$location/content.zip" -PassThru

        $actual.Name | Should -Be "file-1.txt"
        Get-Content "$location/content.zip/file-1.txt" | Should -Be @("line 1", "line 2")

        Test-Path file-1.txt | Should -Be $false
    }
}

Describe "Move-Item zip directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Move-Item Content.zip dir-1" {
        $actual = Move-Item "$location/Content.zip" "$location/dir-1" -PassThru

        $actual.Name  | Should -Be "Content.zip"
        $actual.Parent.Name | Should -Be "dir-1"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Length | Should -Not -Be 0
        $actual.Length | Should -Not -BeNullOrEmpty

        Test-Path $actual | Should -Be $true
        Test-Path "$location/Content.zip" | Should -Be $false
    }

    It "Move-Item Content.zip new.zip" {
        $actual = Move-Item "$location/Content.zip" "$location/new.zip" -PassThru

        $actual.Name  | Should -Be "new.zip"
        $actual.Attributes | Should -Be "File, Archive"

        Test-Path $actual | Should -Be $true
        Test-Path "$location/Content.zip" | Should -Be $false
    }

    It "Move-Item Content.zip existing.zip" {
        New-Item "$location/existing.zip" -ItemType File | Out-Null
        $actual = Move-Item "$location/Content.zip" "$location/existing.zip" -PassThru

        $actual.Name  | Should -Be "existing.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Parent.Name  | Should -Not -Be "existing.zip"

        Test-Path $actual | Should -Be $true
        Test-Path "$location/Content.zip" | Should -Be $false
    }

    It "Move-Item Content.zip existing.zip/" {
        New-Item "$location/existing.zip" -ItemType File | Out-Null
        $actual = Move-Item "$location/Content.zip" "$location/existing.zip/" -PassThru

        $actual.Name  | Should -Be "Content.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Parent.Name  | Should -Be "existing.zip"

        Test-Path $actual | Should -Be $true
        Test-Path "$location/Content.zip" | Should -Be $false
    }

    It "Move-Item Content.zip/* existing" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Move-Item "$location/Content.zip/*" "$location/existing" -PassThru

        (Get-Item "$location/existing/1").Attributes  | Should -Be "Directory"
        (Get-Item "$location/existing/2").Attributes  | Should -Be "Directory"
        (Get-Item "$location/existing/2/2.2").Attributes  | Should -Be "Directory"
        (Get-Item "$location/existing/2/22.txt").Attributes  | Should -Be "File"
        (Get-Item "$location/existing/11.txt").Attributes  | Should -Be "File"
        (Get-Item "$location/existing/inner.zip").Attributes  | Should -Be "File, Archive"

        Test-Path "$location/Content.zip" | Should -Be $true
        Get-ChildItem "$location/Content.zip" | Should -BeNullOrEmpty
    }

    It "Move-Item dir-1 content.zip" {
        Move-Item "$location/dir-1" "$location/content.zip"

        $actual = Get-Item "$location/content.zip/dir-1"
        $actual.Attributes | Should -Be "Directory"
        $actual.Name | Should -Be "dir-1"

        Test-Path dir-1 | Should -Be $false

        (Get-Item "$location/content.zip/dir-1/sub-dir/file1-sub-dir.txt").Attributes | Should -Be "File"
    }

    It "Move-Item dir-1 content.zip/inner.zip" {
        Move-Item "$location/dir-1" "$location/content.zip/inner.zip"

        $actual = Get-Item "$location/content.zip/inner.zip/dir-1"
        $actual.Attributes | Should -Be "Directory"
        $actual.Name | Should -Be "dir-1"

        Test-Path dir-1 | Should -Be $false

        (Get-Item "$location/content.zip/inner.zip/dir-1/sub-dir/file1-sub-dir.txt").Attributes | Should -Be "File"
    }


    It "Move-Item content.zip/2 content.zip/inner.zip/new-2" {
        Move-Item "$location/content.zip/2" "$location/content.zip/inner.zip/new-2"

        $actual = Get-Item "$location/content.zip/inner.zip/new-2"
        $actual.Attributes | Should -Be "Directory"
        $actual.Name | Should -Be "new-2"

        Test-Path "$location/content.zip/2" | Should -Be $false

        (Get-Item "$location/content.zip/inner.zip/new-2/2.2/2.2.txt").Attributes | Should -Be "File"
    }
}
