@echo off
cd %~dp0

SETLOCAL
if ("%ProgramW6432%") == ("") (
	SET dotnet="%ProgramW6432%\dotnet\dotnet.exe"
)
if ("%dotnet%") == ("") (
	SET dotnet="%ProgramFiles%\dotnet\dotnet.exe"
)


IF NOT EXIST %dotnet% (
	echo "Missing installation of dotnet. Download and install from http://dot.net
	exit /b 255
)

:restore
echo Restore packages.
call %dotnet% restore
IF NOT %errorlevel% == 0 (
	@echo !!! Package restore error !!!
	@echo !!! Fix error and rerun !!!
	pause
	exit /b %errorlevel%
)

:run
if exist artifacts (
echo Clean up artifacts folder
del /Q /F /s artifacts
)

echo Create artifacts folder
IF NOT EXIST "artifacts" (
	mkdir "artifacts"
)

set cfg=Release

echo Compile compile (%cfg%) projects.
call %dotnet% build *\project.json --configuration %cfg%

IF NOT %errorlevel% == 0 (
	@echo !!! Build error in %%x !!!
	@echo !!! Fix error and rerun !!!
	pause
	exit /b %errorlevel%
)

echo Packing projects.
call %dotnet% pack MetadataExtractor --configuration %cfg% --output artifacts --no-build
call %dotnet% pack MetadataExtractor-Signed --configuration %cfg% --output artifacts --no-build

IF NOT %errorlevel% == 0 (
	@echo !!! Pack error in %%x !!!
	@echo !!! Fix error and rerun !!!
	pause
	exit /b %errorlevel%
)

echo Running tests.
call %dotnet% test MetadataExtractor.Tests --configuration %cfg% --no-build

IF NOT %errorlevel% == 0 (
	@echo !!! Test error in %%x !!!
	@echo !!! Fix error and rerun !!!
	pause
	exit /b %errorlevel%
)
