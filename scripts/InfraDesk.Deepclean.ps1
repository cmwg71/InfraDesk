# InfraDesk - Deep Clean & Cache Reset Tool
# Dieses Skript löscht alle temporären Build-Dateien und den Visual Studio Cache.

$rootPath = "D:\Projekte\InfraDesk\src"

Write-Host "--- InfraDesk Deep Clean gestartet ---" -ForegroundColor Cyan

# 1. Prüfen, ob Visual Studio noch läuft
$vsProcess = Get-Process -Name "devenv" -ErrorAction SilentlyContinue
if ($vsProcess) {
    Write-Host "WARNUNG: Visual Studio läuft noch. Bitte schließe Visual Studio, damit der .vs-Ordner gelöscht werden kann." -ForegroundColor Yellow
    exit
}

# 2. Verzeichnisse zum Löschen definieren
$foldersToClear = @(".vs", "bin", "obj")

foreach ($folderName in $foldersToClear) {
    Write-Host "Suche nach '$folderName' Ordnern..." -ForegroundColor Gray
    
    # Rekursive Suche nach bin, obj und dem .vs Ordner im Root
    $targets = Get-ChildItem -Path $rootPath -Include $folderName -Recurse -Force -ErrorAction SilentlyContinue | Where-Object { $_.PSIsContainer }

    foreach ($target in $targets) {
        try {
            Write-Host "Lösche: $($target.FullName)" -ForegroundColor White
            Remove-Item -Path $target.FullName -Recurse -Force -ErrorAction Stop
        }
        catch {
            Write-Host "Konnte $($target.FullName) nicht löschen. Eventuell wird der Ordner blockiert." -ForegroundColor Red
        }
    }
}

Write-Host "`n--- Clean abgeschlossen! ---" -ForegroundColor Green
Write-Host "Du kannst die .slnx Datei jetzt wieder mit Visual Studio öffnen." -ForegroundColor White