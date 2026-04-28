## Dateiname: T1_14_Backup_Recovery.md Phase: Stufe 1 (Fundament) Aufgabe: Backup & Wiederherstellung Beschreibung: Implementierung automatisierter Daten-Sicherungen für PostgreSQL. ID: 014

# Aufgabenstellung: Backup & Wiederherstellung

### Beschreibung

Sicherstellung der Datenverfügbarkeit durch automatisierte Sicherungsprozesse der PostgreSQL-Datenbank (inklusive der Binärdaten aus Modul 10).

### Funktionsumfang

1. **Backup-Engine**:
    
    - Integration von `pg_dump` via CLI-Wrapper oder Npgsql-Befehlen.
        
    - Zeitgesteuerte Backups (täglich/wöchentlich) über einen Hintergrunddienst.
        
2. **Speicherort**:
    
    - Konfigurierbarer Pfad für Backup-Dateien (lokal oder Netzwerkfreigabe).
        
    - Implementierung einer Aufbewahrungsrichtlinie (z. B. "Behalte die letzten 7 Tage").
        
3. **Wiederherstellung (Restore)**:
    
    - UI-gestützter Prozess in WinUI, um eine Datenbank aus einem Backup-File wiederherzustellen (erfordert Administrator-Rechte).
        
4. **Logging**:
    
    - Protokollierung des Backup-Status im System-Log.
        

### Abnahmekriterien

- Ein manuell ausgelöstes Backup erzeugt eine valide `.bak` oder `.sql` Datei.
    
- Die Wiederherstellung stellt den Zustand der CMDB, Tickets und Anhänge erfolgreich wieder her.
    
- Fehlgeschlagene Backups lösen eine Benachrichtigung (Modul 09) aus.