## Dateiname: T4_05_CMDB_Federation.md

Phase: Stufe 4 (Expansion & Spezialthemen)

Aufgabe: CMDB Federation & Multi-Source Sync

Beschreibung: Bidirektionaler Abgleich mit Dritt-CMDBs wie ServiceNow.

ID: 045

# Aufgabenstellung: CMDB Federation

### Beschreibung

Vermeidung von Datensilos durch den automatischen Abgleich von Asset-Daten mit anderen Enterprise-Lösungen.

### Funktionsumfang

1. **ServiceNow / Jira Integration**:
    
    - Bidirektionale Synchronisation von CIs (Configuration Items).
        
2. **Konflikt-Regeln**:
    
    - Festlegung des "Master-Systems" pro Attribut (z. B. "Seriennummer kommt immer aus InfraDesk, Standort immer aus ServiceNow").
        
3. **Automatisches Mapping**:
    
    - Intelligente Erkennung gleicher Assets über UUIDs, MAC-Adressen oder Inventarnummern.
        

### Abnahmekriterien

- Änderungen an einem Asset in InfraDesk werden erfolgreich an ein Test-ServiceNow-System übertragen.
    
- Mapping-Profile erlauben eine flexible Attribut-Zuweisung.