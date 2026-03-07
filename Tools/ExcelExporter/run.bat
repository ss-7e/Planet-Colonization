@echo off
setlocal

set "cwd=%~dp0"
echo Working directory: %cwd%
cd /d "%cwd%"

if not exist "venv\" (
    echo Creating virtual environment...
    python -m venv venv
)

echo Installing dependencies...
.\venv\Scripts\pip.exe install -r requirements.txt --disable-pip-version-check

echo Running Excel Exporter...
.\venv\Scripts\python.exe main.py

pause
endlocal
