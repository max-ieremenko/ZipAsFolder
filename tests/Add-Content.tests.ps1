BeforeAll {
    New-TestData -Path $TestDrive
    Push-Location $TestDrive

    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Add-Content" {
    It "Add-Content file-1.txt" {
        Add-Content "$location/file-1.txt" "abc",2
        
        Get-Content file-1.txt | Should -Be @("line 1", "line 2abc", "2")
    }

    It "Add-Content new-file.txt" {
        Add-Content "$location/new-file.txt" "abc",2
        
        Get-Content new-file.txt | Should -Be @("abc", "2")
    }

    It "Add-Content Content.zip/11.txt" {
        Add-Content "$location/Content.zip/11.txt" "abc"
        
        Get-Content "$location/Content.zip/11.txt" | Should -Be @("11abc")
    }

    It "Add-Content Content.zip/new-file.txt" {
        Add-Content "$location/Content.zip/new-file.txt" "abc"
        
        Get-Content "$location/Content.zip/new-file.txt" | Should -Be @("abc")
    }

    It "Add-Content Content.zip/inner.zip/11.txt" {
        Add-Content "$location/Content.zip/inner.zip/11.txt" "abc"
        
        Get-Content "$location/Content.zip/inner.zip/11.txt" | Should -Be @("11abc")
    }

    It "Add-Content Content.zip/inner.zip/new-file.txt" {
        Add-Content "$location/Content.zip/inner.zip/new-file.txt" "abc"
        
        Get-Content "$location/Content.zip/inner.zip/new-file.txt" | Should -Be @("abc")
    }
}