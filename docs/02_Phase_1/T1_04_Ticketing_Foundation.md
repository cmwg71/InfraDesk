## Dateiname: T1_04_Ticketing_Foundation.md Phase: Stufe 1 (Fundament) Aufgabe: Basis-Ticketing & SLAs Beschreibung: Grundgerüst für Incident- und Service-Request Management. ID: 004

# Aufgabenstellung: Ticketing-System (Basis)

### Beschreibung

Aufbau des Ticketing-Moduls zur Erfassung und Bearbeitung von IT-Störungen.

### Funktionsumfang

1. **Ticket-Modell**:
    
    - ID, Titel, Beschreibung (Markdown), Status, Priorität.
        
    - Zuordnung zu Melder (User) und Bearbeiter (Agent).
        
2. **SLA-Logik**:
    
    - Berechnung von `ResponseTime` und `ResolutionTime` basierend auf der Priorität.
        
    - Timer-Dienst zur Überwachung der Schwellenwerte.
        
3. **Kommunikation**:
    
    - Nachrichten-Thread (Journal) innerhalb des Tickets.
        
    - Unterstützung von Dateianhängen (Integration Modul 10).
        
4. **Self-Service**:
    
    - Basis-Endpunkte für das Endbenutzer-Portal.
        

### Abnahmekriterien

- Tickets können erstellt, zugewiesen und geschlossen werden.
    
- SLA-Verletzungen werden im System markiert.
    
- Rich-Text (Markdown) wird korrekt in der UI gerendert.