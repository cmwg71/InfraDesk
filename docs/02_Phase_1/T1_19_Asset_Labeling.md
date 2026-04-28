## Dateiname: T1_19_Asset_Labeling.md

Phase: Stufe 1 (Fundament)

Aufgabe: Inventar-Identifikation & Label-Druck

Beschreibung: Generierung eindeutiger IDs und Druck von QR-Codes für physische Assets.

ID: 019

# Aufgabenstellung: Inventar-Identifikation & Labeling

### Beschreibung

Jedes Asset in der CMDB muss physisch eindeutig identifizierbar sein. Hierzu wird ein System zur Generierung von Inventarnummern und zum Druck von Etiketten implementiert.

### Funktionsumfang

1. **Eindeutige Identifikation**:
    
    - Generierung einer konfigurierbaren Inventarnummer (z.B. `ID-2024-0001`).
        
    - Pflichtfeld-Verknüpfung: Jedes Asset muss einem "Besitzer" (Person) oder einer Kostenstelle zugeordnet sein.
        
2. **QR-Code Generierung**:
    
    - Automatisches Erzeugen von QR-Codes, die einen Deep-Link zur Asset-URL (API/Web) enthalten.
        
3. **Etiketten-Druck**:
    
    - Integration von Druck-Templates für gängige Label-Drucker (z.B. Brother, Zebra).
        
    - Auswahl von Assets in der WinUI-Liste -> "Drucken" (Bulk-Print).
        

### Abnahmekriterien

- Jedes Asset erhält beim Anlegen automatisch eine eindeutige ID.
    
- Ein QR-Code lässt sich als PDF/Bild generieren und ausdrucken.
    
- Die Verknüpfung zur verantwortlichen Person ist im Asset-Detail sichtbar.