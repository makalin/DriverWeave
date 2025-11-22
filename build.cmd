@echo off
echo Checking for .NET SDK...
where dotnet >nul 2>nul
if %errorlevel% neq 0 (
    echo Error: .NET SDK not found in PATH.
    echo Please install the .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
    echo If you have installed it, please restart your terminal or add it to your PATH.
    pause
    exit /b 1
)

echo Building DriverWeave...
dotnet build
if %errorlevel% neq 0 (
    echo Build failed.
    pause
    exit /b 1
)

echo Build successful!
echo You can run the CLI with: src\DriverWeave.Cli\bin\Debug\net8.0\DriverWeave.Cli.exe
echo You can run the App with: src\DriverWeave.App\bin\Debug\net8.0-windows\DriverWeave.App.exe
pause
