@echo off
echo Batch file name: %~nx0
echo Full path: %~f0
for %%A in (%~f0) do (
    echo Last modified: %%~tA
)
pause