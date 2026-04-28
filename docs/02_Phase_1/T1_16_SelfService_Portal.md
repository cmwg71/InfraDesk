## Dateiname: T1_16_SelfService_Portal.md Phase: Stufe 1 (Fundament) Aufgabe: Self-Service-Portal für Endbenutzer Beschreibung: Web-Oberfläche für Mitarbeiter zur Ticket-Erstellung. ID: 016

# Aufgabenstellung: Self-Service-Portal (Web)

### Beschreibung

Entwicklung einer leichtgewichtigen Web-Oberfläche (ASP.NET Core Razor Pages oder Blazor), die in das Backend integriert ist.

### Funktionsumfang

1. **Ticket-Erstellung**:
    
    - Einfaches Formular für Endbenutzer (Titel, Beschreibung, Kategorie).
        
    - Upload-Möglichkeit für Screenshots.
        
2. **Status-Verfolgung**:
    
    - Ansicht der eigenen Tickets mit aktuellem Status und Chat-Verlauf.
        
3. **Authentifizierung**:
    
    - Single Sign-On (SSO) via Entra ID (identisch zu IAM T1_06).
        
4. **Responsive Design**:
    
    - Optimiert für Desktop-Browser und mobile Geräte.
        

### Abnahmekriterien

- Endbenutzer können ohne WinUI-Client Tickets über den Browser eröffnen.
    
- Die Kommunikation zwischen Endbenutzer (Web) und Agent (WinUI) funktioniert bidirektional.