BeforeAll {
    $separator = [System.IO.Path]::DirectorySeparatorChar
}

Describe "Join-Path" {
    It "Join-Path zf:11 22" {
        Join-Path zf:11 22 | Should -Be "zf:11${separator}22"
    }

    It "Join-Path 11 zf:22" {
        # ad designed by PowerShell: implementation of FileSystemProvider.MakePath()
        Join-Path 11 zf:22 | Should -Be "11${separator}zf:22"
    }

    It "Join-Path zf:11 zf:22" {
        Join-Path zf:11 zf:22 | Should -Be "zf:11${separator}22"
    }

    It "Join-Path zf:/11/ zf:22/" {
        Join-Path zf:/11/ zf:22/ | Should -Be "zf:${separator}11${separator}22${separator}"
    }

    It "Join-Path zf:11/ zf:22/" {
        Join-Path zf:11/ zf:22/ | Should -Be "zf:11${separator}22${separator}"
    }

    It "Join-Path zf:11/ zf:22/ 33" {
        Join-Path zf:11/ zf:22/ 33 | Should -Be "zf:11${separator}22${separator}33"
    }

    It "Join-Path zf:11/ zf:22/ zf:33" {
        Join-Path zf:11/ zf:22/ zf:33 | Should -Be "zf:11${separator}22${separator}33"
    }
}