@echo off
setlocal enabledelayedexpansion

set "project_paths=src\Avalonia.MusicStore src\Todo"
set "platforms=win-x64 linux-x64"

call "%~dp0publish.bat" "%project_paths%" "%platforms%"
