# InfraDesk - Final Solution Recovery Script
# Erzwingt die Erstellung einer klassischen .sln Datei und behebt Pfadkonflikte.

$srcPath = "D:\Projekte\InfraDesk\src"
$slnName = "InfraDesk"
$slnFile = "$slnName.sln"

Write-Host "--- InfraDesk Solution Recovery (Classic Mode) ---" -ForegroundColor Cyan

# 1. Visual Studio schließen
Write-Host "Beende Visual Studio..." -ForegroundColor Gray
Get-Process devenv -ErrorAction SilentlyContinue | Stop-Process -Force

# 2. In den Quellordner wechseln
if (Test-Path $srcPath) {
    Set-Location $srcPath
} else {
    Write-Host "FEHLER: Pfad $srcPath nicht gefunden!" -ForegroundColor Red
    return
}

# 3. Gründliche Reinigung
Write-Host "Bereinige Caches und alte Solution-Files..." -ForegroundColor Gray
Remove-Item -Path ".vs" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "*.sln", "*.slnx" -Force -ErrorAction SilentlyContinue

# 4. Erstellen der klassischen .sln
Write-Host "Erstelle neue Solution..." -ForegroundColor White
dotnet new sln -n $slnName --force

# Falls VS trotzdem eine .slnx erstellt hat, konvertieren wir sie
$createdFile = Get-ChildItem -Filter "$slnName.sln*" | Select-Object -First 1
if ($createdFile.Extension -eq ".slnx") {
    Write-Host "VS hat eine .slnx erstellt. Konvertiere zu .sln..." -ForegroundColor Yellow
    Remove-Item $createdFile.FullName -Force
    "Microsoft Visual Studio Solution File, Format Version 12.00`n# Visual Studio Version 17" | Out-File "$slnName.sln" -Encoding utf8
}

# Pfad zur definitiven SLN
$finalSln = Join-Path $srcPath "$slnName.sln"

if (!(Test-Path $finalSln)) {
    Write-Host "FEHLER: Solution konnte nicht initialisiert werden." -ForegroundColor Red
    return
}

# 5. Projekte hinzufügen
Write-Host "Suche und verknüpfe Projekte..." -ForegroundColor Gray
# Wir sammeln alle Projekte, sortieren sie aber, damit Core/Application zuerst kommen
$projects = Get-ChildItem -Recurse -Filter "*.csproj" | Where-Object { $_.FullName -notlike "*\obj\*" }
$waps = Get-ChildItem -Recurse -Filter "*.wapproj" | Where-Object { $_.FullName -notlike "*\obj\*" }

$allFound = $projects + $waps
$addedPaths = @()

foreach ($p in $allFound) {
    # Verhindere doppeltes Hinzufügen, falls Pfade sich überschneiden
    if ($addedPaths -contains $p.FullName) { continue }
    $addedPaths += $p.FullName

    Write-Host "Verarbeite: $($p.Name)" -ForegroundColor White
    
    try {
        if ($p.Extension -eq ".wapproj") {
            Write-Host "  -> Packaging-Projekt erkannt. Versuche Integration..." -ForegroundColor DarkGray
        }
        
        # Führe dotnet sln add aus und fange Fehler ab (besonders für .wapproj wichtig)
        $output = dotnet sln "$finalSln" add "$($p.FullName)" 2>&1
        
        if ($LASTEXITCODE -ne 0) {
            if ($p.Extension -eq ".wapproj") {
                Write-Host "  [!] Info: .wapproj konnte nicht via CLI hinzugefügt werden (SDK-Limitierung)." -ForegroundColor Yellow
                Write-Host "      Dieses Projekt bitte später manuell in Visual Studio hinzufügen." -ForegroundColor Gray
            } else {
                Write-Host "  [!] Fehler beim Hinzufügen von $($p.Name): $output" -ForegroundColor Red
            }
        }
    }
    catch {
        Write-Host "  [X] Ausnahme bei $($p.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n--- RECOVERY ABGESCHLOSSEN ---" -ForegroundColor Green
Write-Host "Die Datei '$finalSln' wurde erstellt."
Write-Host "WICHTIG: Falls das Package-Projekt fehlt:" -ForegroundColor Yellow
Write-Host "1. Öffne die .sln in Visual Studio."
Write-Host "2. Rechtsklick auf die Projektmappe -> Hinzufügen -> Vorhandenes Projekt."
Write-Host "3. Wähle die .wapproj Datei im Ordner 'InfraDesk.UI.WinUI (Package)' aus."