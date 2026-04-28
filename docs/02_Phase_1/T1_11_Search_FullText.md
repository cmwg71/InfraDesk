## Dateiname: T1_11_Search_FullText.md Phase: Stufe 1 (Fundament) Aufgabe: Erweiterte Suche (PostgreSQL) Beschreibung: Implementierung der Volltextsuche über alle Kernmodule. ID: 011

# Aufgabenstellung: Globale Suche (Volltext)

### Beschreibung

Nutzung der PostgreSQL-Features für eine schnelle, modulübergreifende Suche.

### Umsetzung

1. **Such-Index**: Einrichtung von `tsvector`-Spalten in den Tabellen `AssetInstances` (für JSONB-Inhalte) und `Tickets`.
    
2. **Query-Logik**: Implementierung einer Suche, die Teilstrings und gewichtete Ergebnisse (Titel wichtiger als Beschreibung) unterstützt.
    
3. **API**: Ein einziger `/search`-Endpunkt, der Ergebnisse gruppiert nach Typ (Asset, Ticket, IP) zurückgibt.
    

### Abnahmekriterien

- Suchanfragen liefern Ergebnisse in < 200ms bei moderaten Datenbeständen.
    
- Die Suche findet Begriffe innerhalb der dynamischen JSONB-Felder.