BeforeAll {
    New-TestData -Path $TestDrive
    $location = "zf:" + $TestDrive
}

Describe "Test-Path" {
    It "Test-Path zf:dir-1" {
        Test-Path "$location/dir-1" | Should -Be $true
    }

    It "Test-Path zf:not-found" {
        Test-Path "$location/not-found" | Should -Be $false
    }
    
    It "Test-Path zf:not-found -IsValid" {
        Test-Path "$location/not-found" -IsValid | Should -Be $true
    }
    
    It "Test-Path zf:dir-1/sub-dir/* -Exclude *.txt" {
        Test-Path "$location/dir-1/sub-dir/*" -Exclude *.txt | Should -Be $false
    }
    
    It "Test-Path zf:dir-1/* -Exclude *.pdb" {
        Test-Path "$location/dir-1/*" -Exclude *.pdb | Should -Be $true
    }
    
    It "Test-Path zf:dir-1/file1-dir1.txt -PathType leaf" {
        Test-Path "$location/dir-1/file1-dir1.txt" -PathType leaf | Should -Be $true
    }
}