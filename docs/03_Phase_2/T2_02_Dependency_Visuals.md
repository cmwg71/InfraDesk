## Dateiname: T2_02_Dependency_Visuals.md Phase: Stufe 2 (Erweiterte Kernfunktionen) Aufgabe: Visualisierung & Impact-Analyse Beschreibung: Grafische Darstellung der Asset-Abhängigkeiten in WinUI. ID: 022

# Aufgabenstellung: Dependency Visualisierung

### Beschreibung

Grafische Aufbereitung der Asset-Beziehungen, um bei Störungen sofort zu sehen, welche Systeme betroffen sind.

### Funktionsumfang

1. **Interaktiver Graph**:
    
    - Integration einer Graph-Library (z.B. Microsoft Automatic Graph Layout - MSAGL) in WinUI.
        
    - Knoten stellen Assets dar, Kanten die Beziehungen.
        
2. **Impact-Analyse**:
    
    - Funktion: "Was passiert, wenn dieser Server ausfällt?"
        
    - Rekursive Ermittlung aller abhängigen Dienste via CTE (PostgreSQL).
        
3. **Filter**:
    
    - Ein-/Ausblenden bestimmter Beziehungstypen (z.B. nur Netzwerk-Links).
        

### Abnahmekriterien

- Der Graph lässt sich in der WinUI-App flüssig steuern (Zoom/Pan).
    
- Die Impact-Analyse zeigt korrekt alle betroffenen Child-Elemente an.