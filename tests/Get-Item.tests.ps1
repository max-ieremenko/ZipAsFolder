BeforeAll {
    New-TestData -Path $TestDrive
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Get-Item file" {
    It "Get-Item not-found.txt" {
        { Get-Item "$location/not-found.txt" } | Should -Throw
    }

    It "Get-Item file-1.txt" {
        $actual = Get-Item "$location/file-1.txt"

        $actual.Name | Should -Be "file-1.txt"
        $actual.Attributes | Should -Be "File"
    }

    It "Get-Item Content.zip/11.txt" {
        $actual = Get-Item "$location/Content.zip/11.txt"

        $actual.Name | Should -Be "11.txt"
        $actual.Attributes | Should -Be "File"
        $actual.Parent.Name | Should -Be "Content.zip"
    }

    It "Get-Item Content.zip/inner.zip/11.txt" {
        $actual = Get-Item "$location/Content.zip/inner.zip/11.txt"

        $actual.Name | Should -Be "11.txt"
        $actual.Attributes | Should -Be "File"
        $actual.Parent.Name | Should -Be "inner.zip"
    }
}

Describe "Get-Item directory" {
    It "Get-Item" {
        $actual = Get-Item "$location"

        $actual.Name | Should -Be (Get-Item ./).Name
        $actual.Attributes | Should -Be "Directory"
    }

    It "Get-Item dir-1" {
        $actual = Get-Item "$location/dir-1"

        $actual.Name | Should -Be "dir-1"
        $actual.Attributes | Should -Be "Directory"
    }

    It "Get-Item dir-1/sub-dir" {
        $actual = Get-Item "$location/dir-1/sub-dir"

        $actual.Name | Should -Be "sub-dir"
        $actual.Attributes | Should -Be "Directory"
        $actual.Parent.Name | Should -Be "dir-1"
    }

    It "Get-Item content.zip/inner.zip/2" {
        $actual = Get-Item "$location/content.zip/inner.zip/2"

        $actual.Name | Should -Be "2"
        $actual.Attributes | Should -Be "Directory"
        $actual.Parent.Name | Should -Be "inner.zip"
    }
}


Describe "Get-Item zip" {
    It "Get-Item content.zip" {
        $actual = Get-Item "$location/content.zip"

        $actual.Name | Should -Be "Content.zip"
        $actual.Attributes | Should -Be "File, Archive"
    }

    It "Get-Item content.zip/inner.zip" {
        $actual = Get-Item "$location/content.zip/inner.zip"

        $actual.Name | Should -Be "inner.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Parent.Name | Should -Be "Content.zip"
    }
}