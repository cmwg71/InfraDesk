## Dateiname: T2_09_Inventory_Diff_Logic.md

Phase: Stufe 2 (Erweiterte Kernfunktionen)

Aufgabe: Snapshot-Vergleich (Diff-Engine)

Beschreibung: Logik zum Vergleich von Discovery-Snapshots zur Erkennung von Änderungen.

ID: 029

# Aufgabenstellung: Inventory Diff & Historisierung

### Beschreibung

Implementierung einer Engine, die zwei zeitlich versetzte Snapshots eines Assets vergleicht, um Änderungen (z. B. deinstallierte Software, RAM-Upgrade) hervorzuheben.

### Funktionsumfang

1. **Snapshot-Logik**:
    
    - Speicherung von Discovery-Ergebnissen als versionierte Snapshots in der DB.
        
2. **Vergleichs-Algorithmus**:
    
    - Deep-Comparison der JSONB-Strukturen.
        
    - Kategorisierung von Änderungen in: `Hinzugefügt`, `Entfernt`, `Geändert`.
        
3. **UI-Visualisierung**:
    
    - Gegenüberstellung (Side-by-Side) in WinUI mit farblicher Markierung (Grün/Rot/Gelb).
        

### Abnahmekriterien

- Ein Vergleich zeigt präzise an, welche Software-Version sich zwischen zwei Scans geändert hat.
    
- Der Diff-Vorgang für ein Standard-Server-Asset (ca. 500 Attribute) erfolgt in < 100ms.