## Dateiname: T4_02_Monitoring_Integration.md

Phase: Stufe 4 (Expansion & Spezialthemen)

Aufgabe: Integration von Monitoring-Systemen

Beschreibung: Live-Status von PRTG, Zabbix und Checkmk direkt in der CMDB.

ID: 042

# Aufgabenstellung: Monitoring-Integration

### Beschreibung

Integration von Live-Leistungsdaten und Statusmeldungen externer Monitoring-Lösungen direkt in die Asset-Detailansicht der InfraDesk CMDB.

### Funktionsumfang

1. **Multi-Source Support**:
    
    - Konnektoren für **PRTG**, **Zabbix** und **Checkmk** via API.
        
    - Mapping von Monitoring-Sensoren auf CMDB-Assets.
        
2. **Live-Dashboard im Asset**:
    
    - Anzeige von CPU-Last, RAM-Verbrauch und Ping-Zeiten in Echtzeit (via Proxy-Endpunkt).
        
    - Visualisierung von aktiven Alarmen innerhalb der InfraDesk-UI.
        
3. **Event-Koppelung**:
    
    - Automatisierte Ticket-Erstellung (T1_04), wenn ein Monitoring-Sensor einen kritischen Status meldet.
        

### Abnahmekriterien

- In der WinUI-App wird bei einem verknüpften Asset der aktuelle Status aus dem Monitoring-System angezeigt.
    
- Alarme aus PRTG/Zabbix lösen vordefinierte Workflows (T3_02) aus.