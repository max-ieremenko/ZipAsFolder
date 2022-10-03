BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Remove-Item" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Remove-Item not-found.txt" {
        { Remove-Item "$location/not-found.txt" } | Should -Throw
    }

    It "Remove-Item not-found.txt -Force" {
        { Remove-Item "$location/not-found.txt" -Force } | Should -Throw
    }

    It "Remove-Item file-1.txt" {
        Remove-Item "$location/file-1.txt"

        Test-Path file-1.txt | Should -Be $false
        Test-Path dir-1 | Should -Be $true
    }

    It "Remove-Item *.txt" {
        Remove-Item "$location/*.txt"

        Test-Path file-1.txt | Should -Be $false
        Test-Path file-2.txt | Should -Be $false
        Test-Path dir-1 | Should -Be $true
    }

    It "Remove-Item dir-1/* -Recurse" {
        Remove-Item "$location/dir-1/*" -Recurse

        Test-Path dir-1 | Should -Be $true
        Test-Path dir-1/file1-dir1.txt | Should -Be $false
    }

    It "Remove-Item *.txt -Recurse" {
        Remove-Item "$location/*.txt" -Recurse

        Test-Path file-1.txt | Should -Be $false
        Test-Path file-2.txt | Should -Be $false

        # known issue: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/remove-item?view=powershell-7.2#example-4-delete-files-in-subfolders-recursively
        Test-Path dir-1/file1-dir1.txt | Should -Be $true
    }

    It "Get-ChildItem *.txt -Recurse | Remove-Item" {
        Get-ChildItem "$location/*.txt" -Recurse | Remove-Item

        Test-Path file-1.txt | Should -Be $false
        Test-Path file-2.txt | Should -Be $false
        Test-Path dir-1/file1-dir1.txt | Should -Be $false
        Test-Path "$location/Content.zip/11.txt" | Should -Be $false
        Test-Path "$location/Content.zip/2/22.txt" | Should -Be $false
        Test-Path "$location/Content.zip/inner.zip/11.txt" | Should -Be $false
    }

    It "Remove-Item dir-1 -Recurse" {
        Remove-Item "$location/dir-1" -Recurse

        Test-Path file-1.txt | Should -Be $true
        Test-Path dir-1 | Should -Be $false
    }

    It "Remove-Item Content.zip -Recurse" {
        Remove-Item "$location/Content.zip" -Recurse

        Test-Path Content.zip | Should -Be $false
    }

    It "Remove-Item Content.zip/inner.zip -Recurse" {
        Remove-Item "$location/Content.zip/inner.zip" -Recurse

        Test-Path "$location/Content.zip/inner.zip" | Should -Be $false
        Test-Path "$location/Content.zip/11.txt" | Should -Be $true
    }

    It "Remove-Item *.txt -Exclude file-2.txt" {
        Remove-Item "$location/*.txt" -Exclude file-2.txt

        Test-Path file-1.txt | Should -Be $false
        Test-Path file-2.txt | Should -Be $true
    }
}