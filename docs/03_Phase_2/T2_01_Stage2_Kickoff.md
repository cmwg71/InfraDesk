## Dateiname: T2_01_Stage2_Kickoff.md Phase: Stufe 2 (Erweiterte Kernfunktionen) Aufgabe: Stage 2 Initialisierung Beschreibung: Vorbereitung der Automatisierungsschnittstellen und ITSM-Integration. ID: 021

# Aufgabenstellung: Stage 2 Kickoff

### Beschreibung

Nachdem das Fundament (Stufe 1) steht, werden nun die prozessorientierten Module implementiert. Ziel ist die Verknüpfung der CMDB-Daten mit automatisierten Scans und formalen Workflows.

### Fokus-Punkte

1. **Erweiterung des Discovery-Workers**: Implementierung der Schnittstellen für Tiefen-Scans (AD, WMI).
    
2. **Workflow-Basis**: Vorbereitung der State-Machine Logik für Change- und Problem-Management.
    
3. **Relationen-Erweiterung**: Nutzung der in Stufe 1 geschaffenen `AssetLinks` für Impact-Analysen.
    

### Abnahmekriterien

- Entwicklungs-Umgebung für PowerShell-Remoting Tests ist bereitgestellt.
    
- Die Basis-Klassen für Change-Requests sind im Core-Projekt angelegt.