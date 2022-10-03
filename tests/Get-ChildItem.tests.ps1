BeforeAll {
    New-TestData -Path $TestDrive
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Get-ChildItem" {
    It "Get-ChildItem" {
        $actual = Get-ChildItem $location
        $names = $actual | Select-Object -ExpandProperty "Name"

        "file-1.txt" | Should -BeIn $names
        "file-2.txt" | Should -BeIn $names
        "dir-1" | Should -BeIn $names
        "sub-dir" | Should -Not -BeIn $names
        "file1-dir1.txt" | Should -Not -BeIn $names
    }

    It "Get-ChildItem -File" {
        $actual = Get-ChildItem $location -File
        $names = $actual | Select-Object -ExpandProperty "Name"

        "file-1.txt" | Should -BeIn $names
        "file-2.txt" | Should -BeIn $names
        "dir-1" | Should -Not -BeIn $names
        "file1-dir1.txt" | Should -Not -BeIn $names
    }

    It "Get-ChildItem -Directory" {
        $actual = Get-ChildItem $location -Directory
        $names = $actual | Select-Object -ExpandProperty "Name"

        "file-1.txt" | Should -Not -BeIn $names
        "file-2.txt" | Should -Not -BeIn $names
        "dir-1" | Should -BeIn $names
        "file1-dir1.txt" | Should -Not -BeIn $names
    }

    It "Get-ChildItem -Filter f*-1.txt" {
        $actual = Get-ChildItem $location -Filter "f*-1.txt"

        $actual.Name | Should -Be "file-1.txt"
        $actual.Attributes | Should -Be "File"
        $actual.FullName | Should -Be (Join-Path "zf:$TestDrive" "file-1.txt")
        $actual.Extension | Should -Be ".txt"
        $actual.DirectoryName | Should -Be "zf:$TestDrive"
    }

    It "Get-ChildItem -Filter d*-1" {
        $actual = Get-ChildItem $location -Filter "d*-1"

        $actual.Name | Should -Be "dir-1"
        $actual.Attributes | Should -Be "Directory"
        $actual.FullName | Should -Be (Join-Path "zf:$TestDrive" "dir-1")
        $actual.DirectoryName | Should -Be "zf:$TestDrive"
    }

    It "Get-ChildItem Content.zip -Filter *.txt" {
        $actual = Get-ChildItem "$location/Content.zip" -Filter "*.txt"

        $actual.Name | Should -Be "11.txt"
        $actual.Attributes | Should -Be "File"
    }

    It "Get-ChildItem Content.zip/inner.zip -Filter *.txt" {
        $actual = Get-ChildItem "$location/Content.zip/inner.zip" -Filter "*.txt"

        $actual.Name | Should -Be "11.txt"
        $actual.Attributes | Should -Be "File"
    }
}