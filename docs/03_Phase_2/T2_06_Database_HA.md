## Dateiname: T2_06_Database_HA.md Phase: Stufe 2 (Erweiterte Kernfunktionen) Aufgabe: Datenbank-Hochverfügbarkeit Beschreibung: PostgreSQL Cluster-Setup für den Produktivbetrieb. ID: 026

# Aufgabenstellung: PostgreSQL Hochverfügbarkeit

### Beschreibung

Konfiguration einer ausfallsicheren Datenbank-Infrastruktur für Enterprise-Kunden.

### Umsetzung

1. **Streaming Replication**:
    
    - Setup von Primary- und Standby-Nodes.
        
2. **Failover**:
    
    - Implementierung von Patroni oder einer vergleichbaren Lösung zur automatischen Umschaltung.
        
3. **Monitoring**:
    
    - Überwachung der Replikationsverzögerung (Lag) über das Admin-Dashboard.
        

### Abnahmekriterien

- Bei manuellem Stopp des Primary-Nodes übernimmt der Standby-Node automatisch innerhalb von 30 Sekunden.
    
- Die WinUI-App verbindet sich nach einem Failover automatisch wieder mit dem neuen Master.