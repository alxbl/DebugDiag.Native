@echo off
REM * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
REM This script copies the windbg extensions into the DebugDiag install folder.
REM
REM It will check for DebugDiag in the following locations:
REM
REM     %DEBUGDIAG%
REM     C:\Program Files (x86)\DebugDiag\
REM     C:\Program Files\DebugDiag\
REM
REM If your install folder is not one of the above, you can override the install
REM folder by setting the DEBUGDIAG environment variable to the proper path.
REM * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
set IMGDIR=%~dp0
IF NOT "%DEBUGDIAG%"=="" set ENVDD=%DEBUGDIAG%

set DEBUGDIAG=C:\Program Files\DebugDiag

REM 1. Check in Program Files...
echo Checking for %DEBUGDIAG%...
IF NOT EXIST "%DEBUGDIAG%" GOTO TryX86
GOTO CopyFiles


REM 2. Check in Program Files (x86)
:TryX86
set DEBUGDIAG=C:\Program Files (x86)\DebugDiag
echo Checking for %DEBUGDIAG%...
IF NOT EXIST "%DEBUGDIAG%" GOTO TryEnv
GOTO CopyFiles


REM 3. Check for user defined path.
:TryEnv
set DEBUGDIAG=%ENVDD%
echo Checking for `DEBUGDIAG` (%DEBUGDIAG%) environment variable...
set DEBUGDIAG=%ENVDD%
IF NOT EXIST "%DEBUGDIAG%" GOTO Fail
GOTO CopyFiles


:CopyFiles
echo Microsoft Debug Diagnostics located at %DEBUGDIAG%.
echo Copying files from %IMGDIR%...

@echo on
copy "%IMGDIR%redist\NDbgExt.dll" "%DEBUGDIAG%\x86support\Exts\NDbgExt.dll"
copy "%IMGDIR%redist\NDbgExt.pdb" "%DEBUGDIAG%\x86support\Exts\NDbgExt.pdb"
copy "%IMGDIR%redist\NDbgExt64.dll" "%DEBUGDIAG%\Exts\NDbgExt.dll"
copy "%IMGDIR%redist\NDbgExt64.pdb" "%DEBUGDIAG%\Exts\NDbgExt.pdb"
@echo off

echo done.
GOTO Exit

:Fail
echo Microsoft Debug Diagnostics folder not found... abort.

:Exit
pause