## Dateiname: T1_18_API_SDK_Docs.md Phase: Stufe 1 (Fundament) Aufgabe: API SDK & OpenAPI Dokumentation Beschreibung: Bereitstellung von Schnittstellenbeschreibungen für Automatisierungen. ID: 018

# Aufgabenstellung: API-Dokumentation & SDK

### Beschreibung

Professionalisierung der Schnittstellen für interne und spätere externe Automatisierungen.

### Funktionsumfang

1. **OpenAPI (Swagger)**:
    
    - Vollständige Beschreibung aller Endpunkte inklusive DTO-Modellen.
        
    - XML-Kommentare aus dem Code werden in die Dokumentation übernommen.
        
2. **Client-SDK**:
    
    - Automatisierte Generierung eines C#-Clients via NSwag.
        
    - Bereitstellung einer Basis-PowerShell-Bibliothek für Administratoren (Wrapper um die API).
        
3. **API-Sicherheit**:
    
    - Dokumentation der OAuth2-Scopes.
        

### Abnahmekriterien

- Unter `/swagger` findet sich eine vollständige, interaktive Dokumentation.
    
- Ein Test-Skript (PowerShell) kann erfolgreich Assets aus der CMDB lesen.