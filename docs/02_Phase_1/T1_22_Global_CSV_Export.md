## Dateiname: T1_22_Global_CSV_Export.md

Phase: Stufe 1 (Fundament) Aufgabe: Globaler CSV-Export & Datenportabilität Beschreibung: Einheitliche Engine zum Export von Tabellendaten aus allen Modulen. ID: 022

# Aufgabenstellung: Globaler CSV-Export

### Beschreibung

Um die Daten-Souveränität zu gewährleisten, muss jede Tabellenansicht in InfraDesk (Assets, Tickets, IP-Listen, Audit-Logs) über eine Export-Funktion verfügen, die den aktuellen View-Inhalt in eine CSV-Datei überführt.

### Funktionsumfang

1. **Zentraler Export-Service (Backend)**:
    
    - Implementierung einer generischen Export-Logik im `Application`-Layer.
        
    - Unterstützung für flache Listen und komplexe JSONB-Objekte (CMDB-Attribute).
        
    - Automatische Maskierung von Sonderzeichen und Trennern (Delimiters).
        
2. **WinUI Integration**:
    
    - Erstellung eines "Export-Buttons" als wiederverwendbares Control oder Behavior für alle `DataGrid`-Ansichten.
        
    - Integration des Windows-Dateispeicherdialogs zur Wahl des Zielverzeichnisses.
        
3. **Massen-Export**:
    
    - Möglichkeit, gefilterte Suchergebnisse (Modul T1_11) direkt im Bulk zu exportieren.
        
4. **Header-Mapping**:
    
    - Übersetzung von technischen Spaltennamen in menschenlesbare Bezeichner basierend auf der Lokalisierung (i18n).
        

### Technische Umsetzung

- **Bibliothek**: Nutzung von `CsvHelper` (.NET) für performantes Schreiben großer Datenmengen.
    
- **Streaming**: Daten werden gestreamt, um auch bei Exporten von >10.000 Zeilen den Arbeitsspeicher zu entlasten.
    

### Abnahmekriterien

- In jeder Hauptansicht der WinUI-App ist ein Export-Button vorhanden.
    
- Der Export berücksichtigt die aktuell gesetzten Filter der Ansicht.
    
- Sonderzeichen (Umlaute) und Kommata in Feldern werden in der CSV korrekt dargestellt.