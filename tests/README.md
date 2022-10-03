# Tests

## To run tests

- install dependencies

``` powershell
PS> ./build/install-dependencies.ps1
```

- run tests in the current powershell version

``` powershell
PS> cd tests
# run all
PS> ./run-locally.ps1
# run specific
PS> ./run-locally.ps1 .\Copy-Item.tests.ps1, .\Get-ChildItem.tests.ps1
```

- run tests in docker


``` powershell
PS> cd tests
# run all
PS> ./run-indocker.ps1
# run all
PS> ./run-indocker.ps1 -Image mcr.microsoft.com/powershell:7.1.3-ubuntu-20.04
# run specific
PS> ./run-indocker.ps1 .\Copy-Item.tests.ps1, .\Get-ChildItem.tests.ps1
# run specific
PS> ./run-indocker.ps1 -Image mcr.microsoft.com/powershell:7.1.3-ubuntu-20.04 .\Copy-Item.tests.ps1, .\Get-ChildItem.tests.ps1
```