## Dateiname: T1_10_Attachment_Management.md Phase: Stufe 1 (Fundament) Aufgabe: Dateianhänge in PostgreSQL Beschreibung: Speicherung von Binärdaten direkt in der Datenbank. ID: 010

# Aufgabenstellung: DB-basierte Dateiverwaltung

### Beschreibung

Realisierung der Dateispeicherung innerhalb der PostgreSQL-Instanz zur Vereinfachung der Backup-Prozesse.

### Umsetzung

1. **Tabellen-Design**:
    
    - `Attachments`: Metadaten (Name, Typ, Größe, Zuordnung zu Asset/Ticket).
        
    - `AttachmentData`: Spalte `Data` vom Typ `bytea` (getrennt für Performance beim Browsen von Listen).
        
2. **Streaming**:
    
    - Implementierung von Stream-Upload/Download in der API, um den RAM des Servers bei großen Dateien zu schonen.
        
3. **Limits**:
    
    - Konfigurierbare Max-Size (z.B. 20MB pro Datei), um die DB-Größe kontrollierbar zu halten.
        

### Abnahmekriterien

- Dateien werden erfolgreich in die DB geschrieben und daraus gelesen.
    
- Ein Backup der PostgreSQL-DB enthält alle Dokumente.