@echo off
mode con: cols=68 lines=32
title Dreadnought Community Support Tool

setlocal ENABLEDELAYEDEXPANSION

set version=1.0.0
set sourceDirPath=C:\DreadnoughtSupport\
set contentDirPath=%sourceDirPath%\support-info
set targetZipFileName=DN_Support.zip
set destinationDirPath=%userprofile%\Desktop

echo:
echo   ----------------------------------------------------------------
echo    DREADNOUGHT COMMUNITY SUPPORT TOOL
echo   ----------------------------------------------------------------
echo:
echo     This tool will create a zip file of your msinfo, dxdiag and 
echo     log files for bug reporting and submissions when contacting
echo     Customer Support.
echo:
echo     Let the tool run until you see a file on your desktop called
echo     %targetZipFileName%
echo:
echo     Authors:
echo "      - Anyone#6415 <kjarli@gmail.com>"
echo:
echo     For feedback, please contact us on Discord:
echo      - https://discord.gg/dreadnought
echo:
echo     Initial script setup by Spider.
echo:
echo   ----------------------------------------------------------------
echo:
echo     Will create:
echo      - "%sourceDirPath%"
echo:
echo:    Using log directory:
echo      - "%userprofile%\AppData\Local\DreadGame\Saved\Logs"
echo:
echo   ----------------------------------------------------------------
echo:
echo:    Press ENTER to start or close the tool to quit.

pause > nul

rmdir /s /q %sourceDirPath% 2> nul
mkdir %contentDirPath%\logs

echo:
echo     Gathering information, this can take up to a few minutes.
echo     Please don't interrupt or quit the tool while waiting.

set supportReadmeFile=%contentDirPath%\SUPPORT-README.txt

echo Files gathered and zipped by the Dreadnought Community Support Tool (%version%) > %supportReadmeFile%
echo Automatically collected files: >> %supportReadmeFile%
echo  - /logs/*.log >> %supportReadmeFile%
echo  - /dxDiag.txt >> %supportReadmeFile%
echo  - /MSInfo.txt >> %supportReadmeFile%

forfiles /P "%userprofile%\AppData\Local\DreadGame\Saved\Logs" /M *.log /C "cmd /c xcopy @PATH %contentDirPath%\logs 2>&1 > nul"

dxdiag /t %contentDirPath%\DxDiag.txt
Msinfo32 /report %contentDirPath%\MSInfo.txt

set tempFilePath=%temp%\FilesToZip.txt
type nul > %tempFilePath%

FOR /f "delims=*" %%i IN ('dir /b /s /a-d "%sourceDirPath%"') do (
  set filePath=%%i
  set dirPath=%%~dpi
  set dirPath=!dirPath:~0,-1!
  set dirPath=!dirPath:%sourceDirPath%=!
  set dirPath=!dirPath:%sourceDirPath%=!
  echo .set DestinationDir=!dirPath! >> %tempFilePath%
  echo "!filePath!" >> %tempFilePath%
)

makecab /d MaxDiskSize=0 /d CompressionType=MSZIP /d Cabinet=ON /d Compress=ON /d UniqueFiles=ON /d DiskDirectoryTemplate=%destinationDirPath% /d CabinetNameTemplate=%targetZipFileName%  /F %tempFilePath% > nul 2>&1

del setup.inf > nul 2>&1
del setup.rpt > nul 2>&1
del %tempFilePath% > nul 2>&1
echo   ----------------------------------------------------------------
echo:
echo     %targetZipFileName% has been created and is located on your
echo     Desktop. Please include this file as attachment to your
echo     support ticket or email.
echo: 
echo     You can now close the DREADNOUGHT COMMUNITY SUPPORT TOOL!
echo:
echo   ----------------------------------------------------------------
echo:
echo:    Press ENTER to quit.
pause > nul
