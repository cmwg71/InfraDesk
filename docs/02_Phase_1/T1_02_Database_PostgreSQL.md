## Stufe 1 (Fundament) Aufgabe: PostgreSQL, EF Core & Mandantentrennung Beschreibung: DB-Setup inklusive TenantId für alle Tabellen. ID: 002

# Aufgabenstellung: Datenbank-Infrastruktur & Multi-Tenancy

### Beschreibung

Einrichtung der PostgreSQL 16 Datenbank unter Berücksichtigung der Mandantenfähigkeit von Tag 1 an.

### Technische Details

1. **Mandanten-Logik**:
    
    - Einführung einer Tabelle `Tenants`.
        
    - Alle relevanten Tabellen (Assets, Tickets, User) erhalten eine Pflichtspalte `TenantId` (Guid).
        
    - Konfiguration von Global Query Filters in EF Core (`HasQueryFilter`), um Datenlecks zwischen Mandanten zu verhindern.
        
2. **JSONB & Indizes**:
    
    - `AssetInstances.Attributes` als JSONB-Spalte.
        
    - Anlegen eines GIN-Index auf diese Spalte für schnelle Key-Value Suchen.
        
3. **Auditing**:
    
    - Automatische Erfassung von `CreatedBy/At` und `ModifiedBy/At` via DBContext-Override.
        

### Abnahmekriterien

- Datenbank-Schema enthält `TenantId` in allen Entitäten.
    
- Ein Test-Szenario beweist, dass User A keine Daten von Mandant B sehen kann (automatischer Filter).
    
- Migrationen sind versioniert und dokumentiert.