## Dateiname: T1_15_Dashboards_Reports.md Phase: Stufe 1 (Fundament) Aufgabe: Berichte & Dashboards Beschreibung: Visualisierung von KPIs und Asset-Statistiken in der WinUI App. ID: 015

# Aufgabenstellung: Berichte & Dashboards

### Beschreibung

Erstellung einer visuellen Übersicht über den Status der IT-Infrastruktur und die Effizienz des Ticketing-Systems.

### Funktionsumfang

1. **Dashboard-Widgets**:
    
    - Offene Tickets pro Agent/Priorität (Pie-Chart).
        
    - IP-Adress-Belegung (Heatmap).
        
    - Neueste Audit-Ereignisse (Liste).
        
2. **Basis-Berichte**:
    
    - Generierung von Tabellen-Berichten (z. B. "Alle Server mit Ablaufdatum der Garantie").
        
    - Export-Funktion nach Excel (via ClosedXML) und PDF.
        
3. **Technik**:
    
    - Nutzung des `CommunityToolkit.WinUI.UI.Controls.DataGrid` für Tabellen.
        
    - Integration einer Chart-Library für WinUI (z. B. LiveCharts2).
        

### Abnahmekriterien

- Das Dashboard zeigt beim Start aktuelle Echtzeitdaten (via SignalR).
    
- Berichte lassen sich filtern und exportieren.