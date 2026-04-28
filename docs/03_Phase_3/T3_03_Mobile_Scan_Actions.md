## Dateiname: T3_03_Mobile_Scan_Actions.md

Phase: Stufe 3 (Mehrwertfunktionen)

Aufgabe: Mobile Scan & Quick-Actions

Beschreibung: Smartphone-Integration zum Scannen von Assets und Ändern von Status/Standort.

ID: 033

# Aufgabenstellung: Mobile Scan & Quick-Actions

### Beschreibung

Ermöglicht Technikern vor Ort, Assets via Smartphone zu scannen, Informationen abzurufen und sofortige Änderungen vorzunehmen.

### Funktionsumfang

1. **Mobile Scanner**:
    
    - Nutzung der Smartphone-Kamera zum Scannen der QR-Codes (T1_19).
        
2. **Quick-Actions**:
    
    - **Status-Änderung**: Mit einem Klick von "In Lager" auf "Aktiv" setzen.
        
    - **Standort-Update**: Automatische oder manuelle Zuweisung des aktuellen Raums/Standorts.
        
    - **Besitzer-Check**: Anzeige, welcher Person das Asset aktuell zugeordnet ist.
        
3. **API-Integration**:
    
    - Spezialisierte Endpunkte für schnelle Mobile-Updates (minimale Payload).
        

### Abnahmekriterien

- Ein Scan eines QR-Codes öffnet sofort die Asset-Kurzansicht auf dem Smartphone.
    
- Status- und Standortänderungen werden in Echtzeit (SignalR) an den WinUI-Client gemeldet.
    
- Alle mobilen Änderungen werden im Audit-Log mit dem Zusatz "Mobile Scan" vermerkt.