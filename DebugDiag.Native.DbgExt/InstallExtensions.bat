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
@echo off
DEBUGDIAG = "C:\Program Files\DebugDiag\"

REM 1. Check in Program Files...
echo Checking for C:\Program Files\DebugDiag...


REM 2. Check in Program Files (x86)
echo Checking for C:\Program Files (x86)\DebugDiag...

REM 3. Check for user defined path.
echo Checking for `DEBUGDIAG` environment variable...


echo Microsoft Debug Diagnostics folder not found... abort.