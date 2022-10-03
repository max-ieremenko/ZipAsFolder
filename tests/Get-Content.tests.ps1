BeforeAll {
    New-TestData -Path $TestDrive
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Get-Content file" {
    It "Get-Content file-1.txt" {
        $actual = Get-Content "$location/file-1.txt"
        
        $actual | Should -Be @("line 1", "line 2")
    }

    It "Get-Content -Raw file-1.txt" {
        $actual = Get-Content -Raw "$location/zf:file-1.txt"
        
        -split $actual | Should -Be @("line", "1", "line", "2")
    }

    It "Get-Content Content.zip/11.txt" {
        $actual = Get-Content "$location/Content.zip/11.txt"
        
        $actual | Should -Be @("11")
    }

    It "Get-Content Content.zip/inner.zip/11.txt" {
        $actual = Get-Content "$location/Content.zip/inner.zip/11.txt"
        
        $actual | Should -Be @("11")
    }

    It "Get-Content not-found.txt" {
        { Get-Content "$location/not-found.txt" } | Should -Throw
    }
}

Describe "Get-Content directory" {
    It "Get-Content dir-1" {
        { Get-Content "$location/dir-1" } | Should -Throw
    }

    It "Get-Content Content.zip/1" {
        { Get-Content "$location/Content.zip/1" } | Should -Throw
    }
}

Describe "Get-Content zip" {
    It "Get-Content Content.zip" {
        $actual = Get-Content "$location/Content.zip" -ReadCount 1

        $actual | Should -BeTrue
    }

    It "Get-Content Content.zip/inner.zip" {
        $actual = Get-Content "$location/Content.zip/inner.zip" -ReadCount 1

        $actual | Should -BeTrue
    }
}