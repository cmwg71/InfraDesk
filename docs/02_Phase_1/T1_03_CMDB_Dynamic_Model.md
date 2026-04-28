## Dateiname: T1_03_CMDB_Dynamic_Model.md Phase: Stufe 1 (Fundament) Aufgabe: CMDB Core & Dependency Tracking Beschreibung: Dynamische Assets inklusive ihrer Beziehungen. ID: 003

# Aufgabenstellung: Dynamische CMDB & Beziehungen

### Beschreibung

Aufbau des Asset-Katalogs. Da Assets selten isoliert existieren, werden Beziehungen (Dependencies) direkt als Kernfunktion implementiert.

### Funktionsumfang

1. **Klassen-Hierarchie**:
    
    - CRUD für Klassen (z.B. Server < virtuelle Maschine).
        
    - Vererbung von Attributdefinitionen.
        
2. **Asset-Instanzen**:
    
    - Speicherung von Werten in JSONB.
        
    - Status-Tracking (Lifecycle).
        
3. **Beziehungs-Management**:
    
    - Tabelle `AssetLinks`: `SourceAssetId`, `TargetAssetId`, `LinkTypeId` (z.B. "Läuft auf", "Verbunden mit").
        
    - Validierung: Zirkelbezugsprüfung (rekursiv).
        

### UI (WinUI)

- Visualisierung der Beziehungen in einer Baumansicht oder Liste direkt im Asset-Detail.
    

### Abnahmekriterien

- Asset-Klassen und Instanzen können angelegt werden.
    
- Verknüpfungen zwischen Assets sind speicherbar und navigierbar.
    
- Löschweitergabe: Warnung, wenn ein Asset gelöscht wird, das noch Abhängigkeiten hat.