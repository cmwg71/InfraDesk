## Dateiname: T4_04_DLP_Export_Control.md

Phase: Stufe 4 (Expansion & Spezialthemen)

Aufgabe: Data Loss Prevention (DLP)

Beschreibung: Kontrolle und Protokollierung von Massen-Exporten sensibler Daten.

ID: 044

# Aufgabenstellung: Data Loss Prevention (DLP)

### Beschreibung

Schutz vor unkontrolliertem Abfluss sensibler Informationen (z. B. Passwort-Metadaten, Personenbezogene Daten) durch Export-Beschränkungen.

### Funktionsumfang

1. **Export-Regelwerk**:
    
    - Definition von Regeln: "User der Gruppe X darf maximal 100 Assets pro Tag exportieren".
        
    - Blockierung von Exporten, die als "streng vertraulich" markierte Felder enthalten.
        
2. **Genehmigungs-Workflow**:
    
    - Massen-Exporte müssen über den Workflow-Designer (T3_02) durch einen Vorgesetzten freigegeben werden.
        
3. **Erweitertes Audit**:
    
    - Detaillierte Protokollierung jeder exportierten Datei inkl. Prüfsumme.
        

### Abnahmekriterien

- Ein Exportversuch oberhalb der definierten Schwellenwerte wird blockiert und gemeldet.
    
- Alle exportierten CSV-Dateien sind im Audit-Log dem entsprechenden User zugeordnet.