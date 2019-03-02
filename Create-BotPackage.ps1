param(
    [string]$OutputPath,
    [string]$Configuration = "Release"
)
$ErrorActionPreference = "Stop"

$packageRoot = [System.IO.Path]::Combine($Env:TEMP, "KlausPeterBot-package")
if ($OutputPath.Length -gt 0) {
    $packageRoot = $OutputPath
}
$packageBin = [System.IO.Path]::Combine($packageRoot, "bin")
$packagePublish = [System.IO.Path]::Combine($packageBin, "publish")
Write-Host "Creating bot package at $packageRoot"

Write-Host "Building bot application in $Configuration configuration"
dotnet build -c $Configuration .\src\KlausPeterBot\KlausPeterBot.csproj

if (Test-Path -PathType Container -Path $packageRoot) {
    Write-Host "Destination folder already exists. Removing now..."
    Remove-Item -Recurse -Path $packageRoot
}

Write-Host "Creating package folder structure..."
New-Item -ItemType Directory -Path $packageRoot | Out-Null
New-Item -ItemType Directory -Path $packageBin | Out-Null
New-Item -ItemType Directory -Path $packagePublish | Out-Null

Write-Host "Executing dotnet publish"
dotnet publish -c $Configuration -o $packagePublish .\src\KlausPeterBot\KlausPeterBot.csproj

Write-Host "Copying additional script files..."
Copy-Item .\scripts\InstallOrUpdate.sh -Destination $packageRoot
Copy-Item .\klauspeterbot.service -Destination $packageBin

Write-Host "Created package at $packageRoot" -ForegroundColor Green