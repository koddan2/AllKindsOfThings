:: Invoke-Build helper for cmd.exe

@echo off

if "%1"=="/?" goto help

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "submodules\Invoke-Build\Invoke-Build.ps1 %*"
exit /B %errorlevel%

:help
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "help -Full submodules\Invoke-Build\Invoke-Build.ps1"
exit /B 0
