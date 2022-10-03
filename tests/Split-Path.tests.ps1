BeforeAll {
    New-TestData -Path $TestDrive
    $location = "zf:" + $TestDrive
}

Describe "Split-Path" {
    It "Split-Path zf:11 -Qualifier" {
        Split-Path zf:11 -Qualifier | Should -Be "zf:"
    }

    It "Split-Path zf:11 -Leaf" {
        Split-Path zf:11 -Leaf | Should -Be "11"
    }

    It "Split-Path zf:dir-1/*.txt -Leaf -Resolve" {
        Split-Path "$location/dir-1/*.txt" -Leaf -Resolve | Should -Be "file1-dir1.txt"
    }

    It "Split-Path zf:content.zip/1*.txt -Leaf -Resolve" {
        Split-Path "$location/Content.zip/1*.txt" -Leaf -Resolve | Should -Be "11.txt"
    }

    It "Split-Path zf:content.zip/1*.txt" {
        Split-Path zf:content.zip/1*.txt | Should -Be "zf:content.zip"
    }
}