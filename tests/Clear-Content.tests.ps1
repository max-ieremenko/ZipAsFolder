BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Clear-Content file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Clear-Content file-1.txt" {
        Clear-Content "$location/file-1.txt"
        
        Get-Content file-1.txt | Should -Be $null
    }

    It "Clear-Content Content.zip/11.txt -WhatIf" {
        Clear-Content "$location/Content.zip/11.txt" -WhatIf | Out-Null
        
        Get-Content "$location/Content.zip/11.txt" | Should -BeTrue
    }

    It "Clear-Content Content.zip/inner.zip/11.txt" {
        Clear-Content "$location/Content.zip/inner.zip/11.txt"
        
        Get-Content "$location/Content.zip/inner.zip/11.txt" | Should -Be $null
    }

    It "Clear-Content not-found.txt -Force" {
        { Clear-Content "$location/not-found.txt" -Force } | Should -Throw
    }

    It "Clear-Content not-found/not-found.txt -Force" {
        { Clear-Content "$location/not-found/not-found.txt" -Force } | Should -Throw
    }

    It "Clear-Content Content.zip/not-found.txt -Force" {
        { Clear-Content "$location/Content.zip/not-found.txt" -Force } | Should -Throw
    }
}

Describe "Clear-Content directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Clear-Content dir-1 -Force" {
        { Clear-Content "$location/dir-1" -Force } | Should -Throw
    }

    It "Clear-Content Content.zip/1 -Force" {
        { Clear-Content "$location/Content.zip/1" -Force } | Should -Throw
    }
}
Describe "Clear-Content zip" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Clear-Content Content.zip/inner.zip" {
        Clear-Content "$location/Content.zip/inner.zip"

        Test-Path "$location/Content.zip/inner.zip" | Should -Be $true
        Get-ChildItem "$location/Content.zip/inner.zip" | Should -BeNullOrEmpty
    }

    It "Clear-Content Content.zip" {
        Clear-Content "$location/Content.zip"

        Test-Path Content.zip | Should -Be $true
        Get-ChildItem "$location/Content.zip" | Should -BeNullOrEmpty
    }
}