## Dateiname: T1_05_IPAM_Basics.md

Phase: Stufe 1 (Fundament) Aufgabe: IP-Adressverwaltung & DHCP-Monitoring Beschreibung: Verwaltung von Netzen und Überwachung von DHCP-Reservierungen. ID: 005

# Aufgabenstellung: IPAM & DHCP-Logik

### Beschreibung

Zentrale Verwaltung der IP-Infrastruktur mit Fokus auf statische Netze und dedizierte DHCP-Überwachung.

### Funktionsumfang

1. **Netzhierarchie**: Definition von Supernetzen und Subnetzen.
    
2. **DHCP-Bereiche**:
    
    - Statische Definition von DHCP-Scopes.
        
    - **Keine** dynamische Aktualisierung der Range-Größen durch Scans.
        
    - **Reservierungs-Monitor**: Der Discovery-Worker prüft ausschließlich DHCP-Reservierungen und gleicht diese mit der CMDB ab.
        
3. **Lease-Ausschluss**: Kurzzeitige DHCP-Leases werden nicht erfasst.
    
4. **IP-Status**: Frei, Reserviert (statisch), Belegt (Asset-gebunden).
    

### Abnahmekriterien

- DHCP-Reservierungen werden korrekt erkannt und in der IP-Liste angezeigt.
    
- Dynamische Leases tauchen nicht in der permanenten Dokumentation auf.