## Dateiname: T0_14_AutoUpdate_Mechanism.md

Phase: Stufe 0 (Vorbereitung)

Aufgabe: Auto-Update Mechanismus

Beschreibung: Automatisierte Aktualisierung des WinUI-Clients.

ID: 014

# Aufgabenstellung: Client Auto-Updater

### Beschreibung

Da InfraDesk kontinuierlich erweitert wird, muss sichergestellt sein, dass alle Clients auf dem gleichen Stand sind.

### Umsetzung

1. **Versions-Check**:
    
    - Beim Start prüft der WinUI-Client gegen einen API-Endpunkt (`/api/version/latest`), ob eine neuere Version vorliegt.
        
2. **Download & Execute**:
    
    - Falls ein Update vorliegt, lädt die App die neue `setup.exe` (erstellt via Inno Setup) in einen temporären Ordner.
        
    - Die App startet den Installer im Silent-Modus und schließt sich selbst.
        
3. **Update-Server**:
    
    - Einfaches Management-Interface im Backend zum Hochladen neuer Installer-Versionen.
        

### Abnahmekriterien

- Der Nutzer erhält beim Start einen Hinweis, wenn ein Update zwingend erforderlich ist.
    
- Der Update-Vorgang läuft nach Bestätigung vollautomatisch ab.