BeforeAll {
    Push-Location $TestDrive
    $location = "zf:" + $TestDrive
}

AfterAll {
    Pop-Location
}

Describe "Copy-Item file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }

    It "Copy-Item not-found.txt new.txt" {
        { Copy-Item "$location/not-found.txt" "$location/new.txt" } | Should -Throw
    }

    It "Copy-Item file-1.txt not-found/new.txt" {
        { Copy-Item "$location/file-1.txt" "$location/not-found/new.txt" } | Should -Throw
    }

    It "Copy-Item file-1.txt new-file.txt" {
        $actual = Copy-Item "$location/file-1.txt" "$location/new-file.txt" -PassThru

        $actual.Name  | Should -Be "new-file.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }

    It "Copy-Item file-1.txt dir-1" {
        $actual = Copy-Item "$location/file-1.txt" "$location/dir-1" -PassThru

        $actual.Name  | Should -Be "file-1.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }

    It "Copy-Item file-1.txt dir-1 override" {
        New-Item "$location/dir-1/file-1.txt" | Out-Null
        $actual = Copy-Item "$location/file-1.txt" "$location/dir-1" -PassThru

        $actual.Name  | Should -Be "file-1.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }

    It "Copy-Item file-1.txt dir-1/new-file.txt" {
        $actual = Copy-Item "$location/file-1.txt" "$location/dir-1/new-file.txt" -PassThru

        $actual.Name  | Should -Be "new-file.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }

    It "Copy-Item file-1.txt dir-1/file1-dir1.txt" {
        $actual = Copy-Item "$location/file-1.txt" "$location/dir-1/file1-dir1.txt" -PassThru

        $actual.Name  | Should -Be "file1-dir1.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }
    
    It "Copy-Item file-1.txt Content.zip/inner.zip/2" {
        $actual = Copy-Item "$location/file-1.txt" "$location/Content.zip/inner.zip/2" -PassThru

        $actual.Name  | Should -Be "file-1.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }
    
    It "Copy-Item file-1.txt Content.zip/inner.zip/2/new-file.txt" {
        $actual = Copy-Item "$location/file-1.txt" "$location/Content.zip/inner.zip/2/new-file.txt" -PassThru

        $actual.Name  | Should -Be "new-file.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }
    
    It "Copy-Item file-1.txt Content.zip/inner.zip/11.txt" {
        $actual = Copy-Item "$location/file-1.txt" "$location/Content.zip/inner.zip/11.txt" -PassThru

        $actual.Name  | Should -Be "11.txt"
        Test-Path $actual | Should -Be $true
        Test-Path "$location/file-1.txt" | Should -Be $true
    }
}

Describe "Copy-Item directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }
   
    It "Copy-Item dir-1 new" {
        $actual = Copy-Item "$location/dir-1" "$location/new" -PassThru

        $actual.Name  | Should -Be "new"
        $actual.Attributes  | Should -Be "Directory"

        Test-Path $actual | Should -Be $true
        Get-ChildItem $actual | Should -BeNullOrEmpty
    }

    It "Copy-Item dir-1 new -Recurse" {
        Copy-Item "$location/dir-1" "$location/new" -Recurse

        Test-Path "$location/new/file1-dir1.txt" | Should -Be $true
        Test-Path "$location/new/sub-dir/file1-sub-dir.txt" | Should -Be $true
    }

    It "Copy-Item dir-1/* new" {
        Copy-Item "$location/dir-1/*" "$location/new"

        Test-Path "$location/new/file1-dir1.txt" | Should -Be $true
        Test-Path "$location/new/sub-dir" | Should -Be $false
    }

    It "Copy-Item dir-1 existing" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        $actual = Copy-Item "$location/dir-1" "$location/existing" -PassThru

        $actual.Name  | Should -Be "dir-1"
        $actual.Attributes  | Should -Be "Directory"

        Test-Path $actual | Should -Be $true
        Get-ChildItem $actual | Should -BeNullOrEmpty
    }

    It "Copy-Item dir-1 existing -Recurse" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Copy-Item "$location/dir-1" "$location/existing" -Recurse

        Test-Path "$location/existing/dir-1/file1-dir1.txt" | Should -Be $true
        Test-Path "$location/existing/dir-1/sub-dir/file1-sub-dir.txt" | Should -Be $true
    }

    It "Copy-Item dir-1/* existing" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Copy-Item "$location/dir-1/*" "$location/existing"

        Test-Path "$location/existing/file1-dir1.txt" | Should -Be $true
        Test-Path "$location/existing/sub-dir" | Should -Be $true
        Test-Path "$location/existing/sub-dir/file1-sub-dir.txt" | Should -Be $false
    }

    It "Copy-Item dir-1/* existing -Recurse" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Copy-Item "$location/dir-1/*" "$location/existing" -Recurse

        Test-Path "$location/existing/file1-dir1.txt" | Should -Be $true
        Test-Path "$location/existing/sub-dir" | Should -Be $true
        Test-Path "$location/existing/sub-dir/file1-sub-dir.txt" | Should -Be $true
    }

    It "Copy-Item dir-1/* existing -Filter *.bin" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Copy-Item "$location/dir-1/*" "$location/existing" -Filter *.bin

        Get-ChildItem "$location/existing" | Should -BeNullOrEmpty
    }

    It "Copy-Item dir-1/* existing -Filter *.bin -Recurse" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Copy-Item "$location/dir-1/*" "$location/existing" -Filter *.bin -Recurse

        Get-ChildItem "$location/existing" | Should -BeNullOrEmpty
    }
}

Describe "Copy-Item zip file" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }
   
    It "Copy-Item Content.zip/11.txt 11.txt" {
        $actual = Copy-Item "$location/Content.zip/11.txt" "$location/11.txt" -PassThru

        $actual.Name  | Should -Be "11.txt"
        $actual.Attributes  | Should -Be "File"
        Test-Path $actual | Should -Be $true
    }

    It "Copy-Item Content.zip/11.txt new.zip/11.txt" {
        New-Item "$location/new.zip" -ItemType Directory | Out-Null
        $actual = Copy-Item "$location/Content.zip/11.txt" "$location/new.zip/11.txt" -PassThru

        $actual.Name  | Should -Be "11.txt"
        $actual.Attributes  | Should -Be "File"
        Test-Path $actual | Should -Be $true

        Test-Path new.zip | Should -Be $true
    }

    It "Copy-Item Content.zip/11.txt Content.zip/new-11.txt" {
        $actual = Copy-Item "$location/Content.zip/11.txt" "$location/Content.zip/new-11.txt" -PassThru

        $actual.Name  | Should -Be "new-11.txt"
        $actual.Attributes  | Should -Be "File"
        Test-Path $actual | Should -Be $true
    }

    It "Copy-Item Content.zip/11.txt Content.zip/inner.zip/new-11.txt" {
        $actual = Copy-Item "$location/Content.zip/11.txt" "$location/Content.zip/inner.zip/new-11.txt" -PassThru

        $actual.Name  | Should -Be "new-11.txt"
        $actual.Attributes  | Should -Be "File"
        Test-Path $actual | Should -Be $true
    }
}

Describe "Copy-Item zip directory" {
    BeforeEach {
        New-TestData -Path $TestDrive
    }
   
    It "Copy-Item Content.zip dir-1" {
        $actual = Copy-Item "$location/Content.zip" "$location/dir-1" -PassThru

        $actual.Name  | Should -Be "Content.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Length | Should -Not -Be 0
        $actual.Length | Should -Not -BeNullOrEmpty

        Test-Path $actual | Should -Be $true
    }

    It "Copy-Item Content.zip new.zip" {
        $actual = Copy-Item "$location/Content.zip" "$location/new.zip" -PassThru

        $actual.Name  | Should -Be "new.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Length | Should -Not -Be 0
        $actual.Length | Should -Not -BeNullOrEmpty

        Test-Path $actual | Should -Be $true
    }

    It "Copy-Item Content.zip existing.zip" {
        New-Item "$location/existing.zip" -ItemType File | Out-Null
        $actual = Copy-Item "$location/Content.zip" "$location/existing.zip" -PassThru

        $actual.Name  | Should -Be "existing.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Parent.Name  | Should -Not -Be "existing.zip"

        Test-Path $actual | Should -Be $true
    }

    It "Copy-Item Content.zip existing.zip/" {
        New-Item "$location/existing.zip" -ItemType File | Out-Null
        $actual = Copy-Item "$location/Content.zip" "$location/existing.zip/" -PassThru

        $actual.Name  | Should -Be "Content.zip"
        $actual.Attributes | Should -Be "File, Archive"
        $actual.Parent.Name  | Should -Be "existing.zip"

        Test-Path $actual | Should -Be $true
    }

    It "Copy-Item Content.zip/* existing" {
        New-Item "$location/existing" -ItemType Directory | Out-Null
        Copy-Item "$location/Content.zip/*" "$location/existing" -PassThru

        (Get-Item "$location/existing/1").Attributes  | Should -Be "Directory"
        (Get-Item "$location/existing/2").Attributes  | Should -Be "Directory"
        (Get-Item "$location/existing/11.txt").Attributes  | Should -Be "File"
        (Get-Item "$location/existing/inner.zip").Attributes  | Should -Be "File, Archive"

        Get-ChildItem "$location/existing/2" | Should -BeNullOrEmpty
    }
}