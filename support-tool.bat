@echo off
mode con: cols=68 lines=32
title Dreadnought Community Support Tool

SETLOCAL ENABLEDELAYEDEXPANSION

SET version=1.0.0
SET sourceDirPath=C:\DreadnoughtSupport\
SET contentDirPath=%sourceDirPath%\support-info
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
echo     DN_Support.zip
echo:
echo     Authors:
echo      - Anyone#6415 <kjarli@gmail.com>
echo:
echo     For feedback, pease contact us on Discord:
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
echo      - "%USERPROFILE%\AppData\Local\DreadGame\Saved\Logs"
echo:
echo   ----------------------------------------------------------------
echo:
echo:    Press ENTER to start or close the tool to quit.
pause > nul
RMDIR /s /q %sourceDirPath% 2> nul
MKDIR %contentDirPath%\logs
echo:
echo     Gathering information, this can take up to a few minutes.
echo     Please don't interrupt or quit the tool while waiting.

echo Files gathered and zipped by the Dreadnought Community Support Tool (%version%) > %contentDirPath%\SUPPORT-README.txt
echo Automatically collected files: >> %contentDirPath%\SUPPORT-README.txt
echo  - /logs/*.log >> %contentDirPath%\SUPPORT-README.txt
echo  - /DxDiag.txt >> %contentDirPath%\SUPPORT-README.txt
echo  - /MSInfo.txt >> %contentDirPath%\SUPPORT-README.txt

FORFILES /P "%USERPROFILE%\AppData\Local\DreadGame\Saved\Logs" /M *.log /C "cmd /c xcopy @PATH %contentDirPath%\logs 2>&1 > nul"
@echo off
dxdiag /t %contentDirPath%\DxDiag.txt
Msinfo32 /report %contentDirPath%\MSInfo.txt

IF [%2] EQU [] (
  SET destinationDirPath="%USERPROFILE%\Desktop"
) ELSE (
  SET destinationDirPath="%2"
)
IF [%3] EQU [] (
  SET destinationFileName="DN_Support.zip"
) ELSE (
  SET destinationFileName="%3"
)
SET tempFilePath=%TEMP%\FilesToZip.txt
TYPE NUL > %tempFilePath%

FOR /F "DELIMS=*" %%i IN ('DIR /B /S /A-D "%sourceDirPath%"') DO (
  SET filePath=%%i
  SET dirPath=%%~dpi
  SET dirPath=!dirPath:~0,-1!
  SET dirPath=!dirPath:%sourceDirPath%=!
  SET dirPath=!dirPath:%sourceDirPath%=!
  ECHO .SET DestinationDir=!dirPath! >> %tempFilePath%
  ECHO "!filePath!" >> %tempFilePath%
)

MAKECAB /D MaxDiskSize=0 /D CompressionType=MSZIP /D Cabinet=ON /D Compress=ON /D UniqueFiles=ON /D DiskDirectoryTemplate=%destinationDirPath% /D CabinetNameTemplate=%destinationFileName%  /F %tempFilePath% > NUL 2>&1

DEL setup.inf > NUL 2>&1
DEL setup.rpt > NUL 2>&1
DEL %tempFilePath% > NUL 2>&1
echo   ----------------------------------------------------------------
echo:
echo     DN_Support.zip has been created and is located on your
echo     Desktop. Please include this file as attachment to your
echo     support ticket or email.
echo: 
echo     You can now close the DREADNOUGHT COMMUNITY SUPPORT TOOL!
echo:
echo   ----------------------------------------------------------------
echo:
echo:    Press ENTER to quit.
pause > nul
