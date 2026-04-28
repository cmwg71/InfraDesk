## Dateiname: T1_09_Notification_Engine.md Phase: Stufe 1 (Fundament) Aufgabe: Benachrichtigungen & SignalR Echtzeit Beschreibung: Push-Logik via SignalR und E-Mail. ID: 009

# Aufgabenstellung: Echtzeit-Benachrichtigungs-System

### Beschreibung

Implementierung eines Hub-Systems, um den WinUI-Client über Änderungen im Backend zu informieren.

### Technische Umsetzung

1. **SignalR Hubs**:
    
    - `NotificationHub`: Sendet Benachrichtigungen an spezifische User oder Gruppen (z.B. "Alle Agents").
        
2. **Client-Integration (WinUI)**:
    
    - Automatischer Reconnect bei Verbindungsabbruch.
        
    - Anzeige von Windows-Toasts bei eingehenden Nachrichten.
        
3. **Fallback**:
    
    - Wenn der Client offline ist, werden Benachrichtigungen beim nächsten Login als "ungelesen" aus der DB geladen.
        

### Abnahmekriterien

- Ein in der DB erstelltes Ticket erscheint innerhalb von < 1s als Benachrichtigung im Client (ohne Refresh).
    
- Die SignalR-Verbindung ist durch den JWT-Token der API abgesichert.