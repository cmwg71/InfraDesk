## Dateiname: T1_08_JSONB_Validation_Service.md Phase: Stufe 1 (Fundament) Aufgabe: JSONB Validierungs-Service Beschreibung: Typsicherheit für dynamische Felder im Backend. ID: 008

# Aufgabenstellung: JSONB Validierungs-Service

### Beschreibung

Da die PostgreSQL JSONB-Spalte keine feste Struktur vorgibt, muss das Backend sicherstellen, dass die eingegebenen Daten den Definitionen der Asset-Klasse entsprechen.

### Logik

1. **Schema-Abgleich**:
    
    - Vor dem Speichern prüft der Service im `Application`-Layer: Existiert das Feld für diese Klasse?
        
    - Entspricht der Wert dem Datentyp (Zahl, Datum, Regex-String)?
        
2. **Fehlerbehandlung**:
    
    - Rückgabe detaillierter Validierungsfehler an die WinUI App (z.B. "Feld 'RAM' muss eine Zahl sein").
        
3. **Sanitization**:
    
    - Entfernen von Feldern, die nicht in der Klassen-Definition enthalten sind (Data Integrity).
        

### Abnahmekriterien

- Unit-Tests bestätigen, dass ungültige JSON-Strukturen abgelehnt werden.
    
- Pflichtfelder in der CMDB werden korrekt erzwungen.