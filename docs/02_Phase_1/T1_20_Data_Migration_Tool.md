## Dateiname: T1_20_Data_Migration_Tool.md

Phase: Stufe 1 (Fundament)

Aufgabe: Daten-Migration & CSV/Excel Import

Beschreibung: Werkzeuge zur Initialbefüllung der CMDB aus Altdatenbeständen.

ID: 020

# Aufgabenstellung: Migration-Tooling (Import)

### Beschreibung

Um Kunden den Umstieg auf InfraDesk zu erleichtern, wird ein flexibler Import-Assistent benötigt, der Daten aus Excel/CSV in die dynamische CMDB-Struktur mappt.

### Funktionsumfang

1. **Mapping-Editor**:
    
    - UI-Wizard in WinUI zum Zuweisen von CSV-Spalten zu Asset-Attributen.
        
    - Speichern von "Import-Profilen" für wiederkehrende Aufgaben.
        
2. **Validierung-Engine**:
    
    - Vorab-Check: Prüfung der Datentypen und Pflichtfelder vor dem eigentlichen Schreibvorgang.
        
    - Reporting von Fehlern in einer Log-Datei (z. B. "Zeile 45: Datumsformat ungültig").
        
3. **Bulk-Insert**:
    
    - Performantes Schreiben der Assets in die PostgreSQL-Datenbank unter Umgehung der SignalR-Notifications (für Speed).
        

### Abnahmekriterien

- Ein Import von 1.000 Assets aus einer CSV dauert inkl. Validierung < 30 Sekunden.
    
- Das System erkennt Duplikate anhand der Inventarnummer (Modul T1_19).