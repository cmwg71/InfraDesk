## Dateiname: T3_06_Chat_Bot_Integration.md Phase: Stufe 3 (Mehrwertfunktionen) Aufgabe: Chat-Integration (Teams/Slack) Beschreibung: Benachrichtigungen und Interaktionen via Kollaborations-Tools. ID: 036

# Aufgabenstellung: Chat-Bot & Integration

### Beschreibung

Verbindung von InfraDesk mit Microsoft Teams und Slack für proaktive Benachrichtigungen und einfache Befehle.

### Funktionsumfang

1. **Outgoing Webhooks**:
    
    - Automatische Meldung in einen Channel bei "Major Incidents" oder kritischen CMDB-Änderungen.
        
2. **Interaktive Karten (Adaptive Cards)**:
    
    - Genehmigung von Changes oder Service-Requests direkt in Teams (ohne App-Wechsel).
        
3. **Slash-Commands**:
    
    - `/infradesk search [AssetID]` liefert Kurzinfo direkt in den Chat.
        

### Abnahmekriterien

- Eine SLA-Eskalation postet eine Nachricht in einen definierten Teams-Kanal.
    
- Genehmigungen aus dem Workflow-Modul lassen sich über Schaltflächen in der Chat-Nachricht bestätigen.