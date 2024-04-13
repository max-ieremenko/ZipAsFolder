# Build

## To run local build

- install dependencies

[net8.0 sdk](https://dotnet.microsoft.com/download/dotnet/8.0), 
[InvokeBuild](https://www.powershellgallery.com/packages/InvokeBuild/5.9.12), 
[Pester](https://www.powershellgallery.com/packages/Pester/5.3.3)

``` powershell
PS> ./build/install-dependencies.ps1
```

- switch docker to linux containers

- run build

``` powershell
PS> ./build/build.ps1
```

## To test CI build

- switch docker to linux containers

- run build

``` powershell
PS> ./build/test-ci-build.ps1
```
