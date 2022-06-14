@echo off

rem Retrieve steam path from registry
for /F "tokens=2* skip=2" %%a in ('reg query "HKCU\SOFTWARE\Valve\Steam" /v "SteamPath"') do set STEAM_PATH=%%~fb
if exist %STEAM_PATH% (
  echo Steam found at: %STEAM_PATH%
) else (
  echo Steam not found
  pause
  exit 1
)

rem Create links to important locations.
set ROOT=%~dp0
call :link "Linking to Space Engineers"  %ROOT%\SpaceEngineers "%STEAM_PATH%\steamapps\common\SpaceEngineers"
call :link "Linking to workshop items"   %ROOT%\Workshop       "%STEAM_PATH%\steamapps\workshop\content\244850"
call :link "Linking to local mods"       %ROOT%\LocalMods      "%APPDATA%\SpaceEngineers\Mods"

rem For each Mod add a link in SpaceEngineers' local mods folder.
for /D %%i in (%ROOT%\Content\*) do (
  call :link "Adding mod %%~nxi" "%APPDATA%\SpaceEngineers\Mods\%%~nxi" %%i
)

if not exist %ROOT%\SEWhitelistChecker.dll (
  echo:
  echo Downloading WhitePhoera's Space Engineers Whitelist Checker
  curl -LO https://github.com/WhitePhoera/SEWhitelistDiagnostic/releases/download/1.201.013/SEWhitelistChecker.dll
)

echo:
echo Done!
pause
exit

:link
if not exist %2 (
  echo:
  echo %~1
  mklink /d /j %2 %3
)
exit /B 0