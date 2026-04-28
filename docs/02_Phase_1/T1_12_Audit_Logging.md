## Dateiname: T1_12_Audit_Logging.md Phase: Stufe 1 (Fundament) Aufgabe: Audit & Compliance (WORM) Beschreibung: Revisionssichere Protokollierung aller Änderungen. ID: 012

# Aufgabenstellung: Revisionssicheres Audit-Log

### Beschreibung

Jede Änderung an kritischen Daten muss nachvollziehbar sein (ISO 27001 Konformität).

### Umsetzung

1. **Interceptor**: Ein EF Core `SaveChanges`-Interceptor fängt alle Änderungen ab.
    
2. **Payload**: Speicherung von:
    
    - Zeitstempel.
        
    - Benutzer-ID & Tenant-ID.
        
    - Entität & Primärschlüssel.
        
    - Alter Wert vs. Neuer Wert (als JSON).
        
3. **Schutz**: Das Audit-Log darf über die API nur gelesen, niemals geändert oder gelöscht werden.
    

### Abnahmekriterien

- Nach einer Asset-Änderung findet sich ein entsprechender Eintrag in der `AuditLogs`-Tabelle.
    
- Das Audit-Log ist über eine Admin-Ansicht in WinUI filterbar.