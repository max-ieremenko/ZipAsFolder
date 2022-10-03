BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Set-Content file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Set-Content file-1.txt" {
        Set-Content "$location/file-1.txt" "abc", 2
        
        Get-Content file-1.txt | Should -Be @("abc", "2")
    }

    It "Set-Content new-file.txt" {
        Set-Content "$location/new-file.txt" "abc", 2
        
        Get-Content new-file.txt | Should -Be @("abc", "2")
    }

    It "Set-Content Content.zip/11.txt -WhatIf" {
        Set-Content "$location/Content.zip/11.txt" -WhatIf "abc", 2 | Out-Null
        
        Get-Content "$location/Content.zip/11.txt" | Should -Be @("11")
    }

    It "Set-Content Content.zip/new-file.txt" {
        Set-Content "$location/Content.zip/new-file.txt" "abc", 2
        
        Get-Content "$location/Content.zip/new-file.txt" | Should -Be @("abc", "2")
    }

    It "Set-Content Content.zip/inner.zip/11.txt" {
        Set-Content "$location/Content.zip/inner.zip/11.txt" "new text"
        
        Get-Content "$location/Content.zip/inner.zip/11.txt" | Should -Be "new text"
    }

    It "Set-Content Content.zip/inner.zip/new-file.txt" {
        Set-Content "$location/Content.zip/inner.zip/new-file.txt" "new text"
        
        Get-Content "$location/Content.zip/inner.zip/new-file.txt" | Should -Be "new text"
    }
}

Describe "Set-Content directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Set-Content dir-1" {
        { Set-Content "$location/dir-1" "abc" } | Should -Throw
    }

    It "Set-Content Content.zip/1" {
        { Set-Content "$location/Content.zip/1" "abc" } | Should -Throw
    }
}

Describe "Set-Content directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Set-Content Content.zip" {
        # because of reset-content, the value.length must be > 22.bytes
        Set-Content "$location/Content.zip" "1234567891011121314151617"

        Get-Content "$location/Content.zip" | Should -Be "1234567891011121314151617"
    }

    It "Set-Content Content.zip/inner.zip" {
        Set-Content "$location/Content.zip/inner.zip" "1234567891011121314151617"

        Get-Content "$location/Content.zip/inner.zip" | Should -Be "1234567891011121314151617"
    }

    It "Set-Content new.zip" {
        Set-Content "$location/new.zip" "1234567891011121314151617"

        Get-Content "$location/new.zip" | Should -Be "1234567891011121314151617"
    }
}