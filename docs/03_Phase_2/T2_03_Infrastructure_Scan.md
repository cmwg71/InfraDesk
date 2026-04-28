## Dateiname: T2_03_Infrastructure_Scan.md Phase: Stufe 2 (Erweiterte Kernfunktionen) Aufgabe: Automatisierte Server-Dokumentation Beschreibung: Tiefen-Scan von Windows-Systemen und AD-Strukturen via Worker. ID: 023

# Aufgabenstellung: Infrastruktur-Tiefenanalyse

### Beschreibung

Automatisierte Datenerfassung ohne manuelle Pflege der CMDB durch den Discovery-Worker (T1_07).

### Scan-Module

1. **Windows-Systeme**:
    
    - Abfrage via WMI/CIM: CPU, RAM, Software-Liste, Patch-Level, BitLocker-Status.
        
2. **Active Directory**:
    
    - Forest-Struktur, OUs, FSMO-Rollen, GPO-Verknüpfungen.
        
3. **Netzwerk-Dienste**:
    
    - DNS-Records und DHCP-Scope Auslastung.
        

### Workflow

- Worker führt Scan aus -> Sendet JSON-Payload an API -> API aktualisiert Asset-Attribute oder erstellt neue Assets (Staging-Prozess).
    

### Abnahmekriterien

- Ein Test-Server wird innerhalb von < 2 Minuten vollständig inventarisiert.
    
- Änderungen (z.B. neue Software) werden als neue Version im Audit-Log vermerkt.