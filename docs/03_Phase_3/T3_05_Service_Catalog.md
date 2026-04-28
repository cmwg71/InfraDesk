## Dateiname: T3_05_Service_Catalog.md Phase: Stufe 3 (Mehrwertfunktionen) Aufgabe: Servicekatalog & Self-Service Beschreibung: Standardisierte IT-Services für Endanwender mit Bestellprozess. ID: 035

# Aufgabenstellung: Servicekatalog & Bestellwesen

### Beschreibung

Endanwender können über das Web-Portal (T1_16) definierte IT-Services (z. B. "Neuer Monitor", "VPN-Zugang") anfordern.

### Funktionsumfang

1. **Service-Definition**:
    
    - Erstellung von Service-Karten mit Beschreibung, Bild und SLA.
        
2. **Bestell-Formulare**:
    
    - Dynamische Formulare basierend auf dem Service-Typ (z. B. Auswahl des Monitor-Modells).
        
3. **Workflow-Kopplung**:
    
    - Jede Bestellung startet automatisch einen Workflow (T3_02) zur Genehmigung durch den Vorgesetzten.
        
4. **Integration**:
    
    - Nach Lieferung wird das Asset automatisch in der CMDB dem User zugewiesen.
        

### Abnahmekriterien

- Ein User kann einen Service bestellen, der einen Genehmigungs-Task für einen Admin erzeugt.
    
- Die Historie der Bestellung ist im Self-Service-Portal einsehbar.