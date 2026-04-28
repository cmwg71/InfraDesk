## Dateiname: T1_07_Discovery_Worker_Setup.md Phase: Stufe 1 (Fundament) Aufgabe: Architektur des Discovery-Workers Beschreibung: Setup für den Hintergrunddienst zur Infrastruktur-Erfassung. ID: 007

# Aufgabenstellung: Discovery-Worker (Architektur)

### Beschreibung

Planung und Initialisierung des "Discovery-Workers", der die agentlose Erfassung (WMI/PowerShell) durchführt.

### Komponenten

1. **Worker-Dienst**: Ein leichtgewichtiger .NET 8 Windows Service.
    
2. **Kommunikation**:
    
    - Der Worker holt sich "Jobs" vom Hauptserver ab.
        
    - Ergebnisse werden verschlüsselt an das Backend zurückgegeben.
        
3. **Sicherheit**:
    
    - Der Worker läuft im Kontext eines Service-Accounts mit den nötigen Berechtigungen (z.B. Domain Admin oder dedizierter Scan-User).
        

### Erste Scan-Module

- Ping-Scan (ICMP) zur Erkennung aktiver Geräte im Subnetz.
    
- Hostname-Auflösung via DNS.
    

### Abnahmekriterien

- Worker kann erfolgreich zum Backend connecten.
    
- Scan-Ergebnisse werden korrekt in die CMDB-Staging-Tabelle übertragen.