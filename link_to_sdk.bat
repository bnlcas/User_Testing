:: Prevent garbage in the terminal
@echo off

:: Check if folder exist
IF EXIST Assets\MetaSDK (
    echo ">> MetaSDK folder already exists"
    EXIT
)

:: This script is executed twice if no admin rights are established.
:: The first time it runs it ask for administrative privileges and then re-execute this script in the new admin terminal
:: The second time it runs as an admin
:: Check for Admin privileges
:: -------------------------------------
:: Check for permissions
IF "%PROCESSOR_ARCHITECTURE%" EQU "amd64" (
    >nul 2>&1 "%SYSTEMROOT%\SysWOW64\cacls.exe" "%SYSTEMROOT%\SysWOW64\config\system"
) ELSE (
    >nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
)

:: Error flag set, we do not have admin.
IF '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) ELSE (
    goto GotAdmin
)

:: Request for Admin privileges.
:: It launch a new terminal as admin mode and run this script again.
:: After launching the new terminal it exit this script.
:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params = %*:"=""
    echo UAC.ShellExecute "cmd.exe", "/c ""%~s0"" %params%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    del "%temp%\getadmin.vbs"
    exit /B

:: This section ensures that in the new terminal, if any, we execute this script in the original folder.
:: Failing to do this, will execute the script in the system folder.
:GotAdmin
    pushd "%CD%"
    CD /D "%~dp0"
:: --------------------------------------

:: This section runs in Admin mode only
:: Create symbolic link to SDK
echo ">> Creating symbolic link to MetaSDK"
mklink /D Assets\MetaSDK %META_INTERNAL_UNITY_SDK%