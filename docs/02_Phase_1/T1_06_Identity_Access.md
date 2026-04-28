## Dateiname: T1_06_Identity_Access.md Phase: Stufe 1 (Fundament) Aufgabe: Identity & Access Management Beschreibung: Absicherung des Systems via Entra ID und RBAC. ID: 006

# Aufgabenstellung: IAM & Berechtigungen

### Beschreibung

Implementierung der Sicherheitsschicht für Authentifizierung und Autorisierung.

### Funktionsumfang

1. **Authentifizierung**:
    
    - OpenID Connect Integration für Entra ID (Azure AD).
        
    - Lokaler Login-Fallback für Notfall-Admins.
        
2. **RBAC (Role-Based Access Control)**:
    
    - Definition von Rollen (Admin, Agent, User).
        
    - Feingranulare Berechtigungen (z.B. `CanReadCMDB`, `CanDeleteTickets`).
        
3. **Audit**:
    
    - Protokollierung jedes Login-Versuchs.
        

### Abnahmekriterien

- Zugriff auf API-Endpunkte ist ohne gültigen Token gesperrt.
    
- UI zeigt Module nur basierend auf den Benutzerrechten an.
    
- Erfolgreicher Login via Microsoft-Konto.