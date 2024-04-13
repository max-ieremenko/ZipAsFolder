$ProgressPreference = 'SilentlyContinue'

Expand-Archive /module.zip /root/.local/share/powershell/Modules/ZipAsFolder

/app/scripts/Run-TestsDocker.ps1 -Output Minimal