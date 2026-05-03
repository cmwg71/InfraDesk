**Phase:** Stufe 2 (Erweiterte Kernfunktionen)  
**Aufgabe:** Visualisierung & Impact-Analyse  
**ID:** 022  
**Status:** Enterprise-Ready

Aufgabenstellung: Dependency Visualisierung & Root-Cause-Analysis

1. Beschreibung

Implementierung einer interaktiven, grafischen Oberfläche zur Darstellung von IT-Beziehungen (Relationship Mapping). Das Tool dient Technikern zur schnellen Identifikation von Fehlerquellen (Root-Cause) und zur Bewertung von Risiken bei geplanten Wartungen (Impact-Analysis).

2. Funktionsumfang (Enterprise-Standard)

2.1 Dynamische Graph-Engine

- **Graph-Logik:** Nutzung einer hochperformanten Engine (z.B. MSAGL oder WinUI-spezifische Wrapper für D3-ähnliche Layouts).
- **Layer-Modell:** Automatische Anordnung der Knoten in Ebenen (z.B. Business Service -> Applikation -> Server -> Storage -> Netzwerk).
- **Clustering:** Logische Gruppierung von Instanzen (z.B. VMware-Cluster oder Datenbank-Verbund), um Komplexität in der Standardansicht zu reduzieren.

2.2 Reale Impact-Analyse (Rekursions-Logik)

- **Upstream/Downstream-Check:**
    - _Downstream:_ Welche Services fallen aus, wenn dieser Switch stirbt?
    - _Upstream:_ Welcher physische Host verursacht den Ausfall dieser virtuellen SQL-Instanz?
- **Redundanz-Erkennung:** Intelligente Berechnung: Wenn ein Asset in einem HA-Cluster ausfällt, wird die Verbindung zum Dienst nur als "Warnung" (gelb) statt "Kritisch" (rot) markiert, solange die Redundanz hält.

2.3 Integration in das Ticketing (ITSM-Koppelung)

- **Ticket-Overlay:** Anzeige aktiver Incidents direkt am Icon des Assets im Graphen.
- **Change-Simulation:** "Wartungsmodus-Vorschau" – Markierung eines Knotens zeigt sofort alle betroffenen Endanwender und Business-Services an, die informiert werden müssen (Modul T1_09).

3. Technische Umsetzung

3.1 Backend-Logik (PostgreSQL CTE)

- **Recursive Queries:** Nutzung von _Common Table Expressions_ (CTE), um den Baum der Beziehungen tiefen-unabhängig in einer einzigen DB-Abfrage aufzulösen.
- **Relationship Types:** Unterscheidung zwischen physischen (`connected to`), logischen (`runs on`) und administrativen (`managed by`) Relationen.

3.2 WinUI UI/UX Features

- **Semantic Zoom:** Detailgrad der Informationen am Knoten passt sich der Zoomstufe an (von "Nur Icon" bis "Vollständige Specs").
- **Export-Funktion:** Generierung von Topologie-Diagrammen als PDF/SVG für Dokumentationszwecke oder Audits.

4. Workflow (Szenario Störung)

5. **Meldung:** Ein Ticket für "Applikation ERP" geht ein.
6. **Visualisierung:** Der Agent öffnet den Dependency-Graph der Applikation.
7. **Analyse:** Das System markiert einen zugrunde liegenden Switch rot, da dieser einen SNMP-Alert (T2_03) gesendet hat.
8. **Impact:** Der Agent sieht sofort: "ERP ist down, aber auch das Backup-System ist betroffen".

9. Abnahmekriterien

- **Performance:** Graph-Aufbau für eine Umgebung mit 500 Knoten erfolgt in < 2 Sekunden.
- **Rekursions-Tiefe:** Die Impact-Analyse verfolgt Abhängigkeiten über mindestens 10 Ebenen fehlerfrei.
- **Interaktivität:** Knoten lassen sich manuell verschieben (Layout-Lock), und Kanten zeigen bei Mouse-over den Beziehungstyp an.
- **Filter-Präzision:** Das Ausblenden von "Soft-Links" (z.B. administrativer Besitzer) lässt nur die technisch kritischen Pfade übrig.