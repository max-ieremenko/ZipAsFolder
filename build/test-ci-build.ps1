#Requires -Version "7.0"

$context = Join-Path $PSScriptRoot "../"

$containerId = docker run -it -d `
    -v "$($context):/app" `
    --entrypoint pwsh `
    mcr.microsoft.com/dotnet/sdk:6.0.401-focal-amd64 `
    -NoExit

docker exec -it $containerId pwsh /app/build/install-dependencies.ps1
docker exec -it $containerId pwsh /app/build/build.ps1 CIBuild
docker rm -f $containerId | Out-Null