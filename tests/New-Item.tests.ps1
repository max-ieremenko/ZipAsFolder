BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "New-Item file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "New-Item new-file.txt -Value abc" {
        $actual = New-Item "$location/new-file.txt" -Value "abc", 2
        $actual.Attributes | Should -Be "File"
        
        Get-Content new-file.txt | Should -Be @("abc", "2")
    }

    It "New-Item new-file.txt/" {
        $actual = New-Item "$location/new-file.txt/"
        $actual.Attributes | Should -Be "File"
        
        Test-Path new-file.txt | Should -Be $true
    }

    It "New-Item file-1.txt" {
        { New-Item "$location/file-1.txt" } | Should -Throw
    }

    It "New-Item file-1.txt -Force -Value abc" {
        New-Item "$location/file-1.txt" -Force -Value "abc", 2

        Get-Content file-1.txt | Should -Be @("abc", "2")
    }

    It "New-Item 11/11.zip/inner.zip/file-1.txt" {
        $actual = New-Item "$location/11/11.zip/inner.zip/file-1.txt" -Value "abc", 2
        
        Get-Content $actual | Should -Be @("abc", "2")

        (Get-Item "$location/11/11.zip").Attributes | Should -Be "Directory"
        (Get-Item "$location/11/11.zip/inner.zip").Attributes | Should -Be "Directory"
    }

    It "New-Item what-if-file.txt" {
        New-Item "$location/what-if-file.txt" -WhatIf
        
        Test-Path what-if-file.txt | Should -Be $false
    }
}

Describe "New-Item directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "New-Item new-folder -ItemType Directory" {
        $actual = New-Item "$location/new-folder" -ItemType Directory

        $actual.Attributes | Should -Be "Directory"
        
        Test-Path new-folder | Should -Be $true
    }

    It "New-Item folder/folder2 -ItemType Directory" {
        $actual = New-Item "$location/folder/folder2" -ItemType Directory

        $actual.Attributes | Should -Be "Directory"
        
        Test-Path folder/folder2 | Should -Be $true
    }
    
    It "New-Item dir-1 -ItemType Directory" {
        { New-Item "$location/dir-1" -ItemType Directory } | Should -Throw
    }
    
    It "New-Item dir-1 -ItemType Directory -Force" {
        $actual = New-Item "$location/dir-1"  -ItemType Directory -Force
        $actual.Attributes | Should -Be "Directory"
    }
}

Describe "New-Item zip file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "New-Item new.zip" {
        $actual = New-Item "$location/new.zip"

        $actual.Attributes | Should -Be "File, Archive"
        Test-Path $actual | Should -Be $true

        (Get-ChildItem new.zip).GetType().Name | Should -Be "FileInfo"
    }

    It "New-Item new.zip -ItemType Directory" {
        $actual = New-Item "$location/new.zip" -ItemType Directory

        $actual.Attributes | Should -Be "Directory"
        Test-Path $actual | Should -Be $true

        (Get-Item new.zip).GetType().Name | Should -Be "DirectoryInfo"
    }

    It "New-Item Content.zip/new-1.zip/new-2.zip" {
        $actual = New-Item "$location/Content.zip/new-1.zip/new-2.zip"

        $actual.Attributes | Should -Be "File, Archive"
        Test-Path $actual | Should -Be $true

        (Get-Item "$location/Content.zip/new-1.zip").Attributes | Should -Be "Directory"
    }

    It "New-Item Content.zip/new-1.zip/new-2.zip -ItemType Directory" {
        $actual = New-Item "$location/Content.zip/new-1.zip/new-2.zip" -ItemType Directory

        $actual.Attributes | Should -Be "Directory"
        Test-Path $actual | Should -Be $true

        (Get-Item "$location/Content.zip/new-1.zip").Attributes | Should -Be "Directory"
    }
}