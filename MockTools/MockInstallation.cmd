@echo off
setlocal

set "WAIT_SECONDS=%~1"
if "%WAIT_SECONDS%"=="" set "WAIT_SECONDS=8"

set "RESULT=%~2"
if "%RESULT%"=="" set "RESULT=pass"

set "RESULT_NORMALIZED=%RESULT%"
for %%A in ("%RESULT_NORMALIZED%") do set "RESULT_NORMALIZED=%%~A"

echo [MockInstallation] Starting installation mock. Wait=%WAIT_SECONDS%s Result=%RESULT_NORMALIZED%
timeout /t %WAIT_SECONDS% /nobreak >nul

if /I "%RESULT_NORMALIZED%"=="pass" exit /b 1
if /I "%RESULT_NORMALIZED%"=="success" exit /b 1
if "%RESULT_NORMALIZED%"=="1" exit /b 1

if /I "%RESULT_NORMALIZED%"=="fail" exit /b 0
if /I "%RESULT_NORMALIZED%"=="failed" exit /b 0
if "%RESULT_NORMALIZED%"=="0" exit /b 0

echo [MockInstallation] Unknown result value '%RESULT_NORMALIZED%'. Defaulting to failure.
exit /b 0
