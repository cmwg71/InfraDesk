# InfraDesk - Ultra Clean & Build Repair (Erzwingt .NET 8 SDK)
$rootPath = "D:\Projekte\InfraDesk\src"
$globalJsonPath = "D:\Projekte\InfraDesk\global.json"

Write-Host "--- Beende ALLE blockierenden Prozesse ---" -ForegroundColor Cyan
$processes = @("devenv", "MSBuild", "VBCSCompiler", "InfraDesk", "dotnet", "ServiceHub.*")
foreach($p in $processes) {
    Get-Process -Name $p -ErrorAction SilentlyContinue | Stop-Process -Force
}

Start-Sleep -Seconds 2

# SDK FIXIERUNG AUF .NET 8
Write-Host "--- Erzwinge .NET 8 via global.json ---" -ForegroundColor White
$globalJsonContent = @"
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMinor"
  }
}
"@

try {
    $globalJsonContent | Out-File -FilePath $globalJsonPath -Encoding utf8 -Force
    Write-Host ".NET 8 wurde in der global.json erfolgreich fixiert." -ForegroundColor Green
}
catch {
    Write-Host "FEHLER: global.json konnte nicht erstellt werden!" -ForegroundColor Red
}

Write-Host "--- Lösche Cache-Ordner ---" -ForegroundColor Yellow
$folders = @("bin", "obj", ".vs", "Generated Files")
foreach($f in $folders) {
    Write-Host "Bereinige $f..." -ForegroundColor Gray
    Get-ChildItem -Path $rootPath -Recurse -Directory -Filter $f -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
}

# Verifikation
Write-Host "--- Aktives SDK (Sollte jetzt mit 8.0 beginnen) ---" -ForegroundColor White
dotnet --version

Write-Host "--- Starte sauberen Restore & Build via CLI ---" -ForegroundColor Green
cd $rootPath
dotnet restore
dotnet build --no-restore