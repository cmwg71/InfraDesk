**Phase:** Stufe 2 (Erweiterte Kernfunktionen)  
**Aufgabe:** Change & Problem Management (ITIL-Governance)  
**ID:** 025  
**Status:** Enterprise-Ready

Aufgabenstellung: Strukturierte ITIL-Prozesse (Governance)

1. Beschreibung

Erweiterung des Basisticketings (T1_04) um strategische ITIL-Disziplinen. Ziel ist die Minimierung von Risiken bei Infrastrukturänderungen (Change) und die nachhaltige Beseitigung von Fehlerursachen (Problem), anstatt nur Symptome zu bekämpfen.

2. Change Management (Kontrollierte Änderungen)

2.1 Request for Change (RFC) & Risikobewertung

- **Klassifizierung:** Unterscheidung in _Standard-Change_ (vorab genehmigt), _Normal-Change_ (erfordert Review) und _Emergency-Change_ (beschleunigt).
- **Impact-Visualisierung:** Direkte Einbindung des Dependency-Graphs (T2_02), um bei einem Change sofort zu sehen: "Welche Business-Services sind während der Wartung offline?".
- **Planung:** Erfassung von Wartungsfenstern (Downtime), Rollback-Plänen und Test-Ergebnissen.

2.2 Change Advisory Board (CAB) & Genehmigung

- **Workflow-Steuerung:** Automatisierte Zuweisung an das CAB basierend auf der Kritikalität der betroffenen CIs.
- **Digital Sign-off:** Revisionssichere Dokumentation der Freigabe durch den Change Manager (Modul T1_12).
- **Kollisionsprüfung:** Warnung, wenn zwei Changes gleichzeitig am selben Asset oder im selben Netzwerksegment geplant sind.

3. Problem Management (Ursachenanalyse)

3.1 Root Cause Analysis (RCA)

- **Incident-Aggregation:** Gruppierung von wiederkehrenden oder massiven Incidents zu einem Problem-Ticket.
- **Trend-Analyse:** Dashboard zur Identifikation von "Sorgenkind-CIs" (Assets mit hoher Incident-Dichte).
- **Dokumentation:** Strukturierte Erfassung der Fehlerursache (Technisch, Prozessual, Menschlich).

3.2 Known Error Database (KEDB)

- **Workaround-Publishing:** Sofortige Bereitstellung von Übergangslösungen für den Service Desk.
- **Proaktive Suche:** Bei Erstellung eines neuen Incidents schlägt das System via Volltextsuche (T1_11) automatisch existierende Workarounds aus der KEDB vor.

4. Workflow-Integration

5. **Problem identifiziert:** Mehrere User melden "ERP langsam". Ein Problem-Ticket wird erstellt.
6. **Lösung gefunden:** Ursache ist ein veralteter DB-Index. Lösung erfordert ein Update.
7. **RFC ausgelöst:** Aus dem Problem heraus wird ein _Request for Change_ generiert.
8. **Change-Execution:** Nach CAB-Freigabe wird der Change durchgeführt, erfolgreich getestet und geschlossen.
9. **Prozess-Abschluss:** Das System schließt automatisch das Problem-Ticket und alle damit verknüpften Incidents (Auto-Closure).

10. Abnahmekriterien

- **Prozess-Sperre:** Ein Normal-Change kann technisch nicht auf "In Umsetzung" gesetzt werden, ohne dass eine Genehmigung (Approval) im System vorliegt.
- **KEDB-Integration:** Ein im Problem hinterlegter Workaround wird in der Ticket-Ansicht des Service Desks als "Vorgeschlagene Lösung" eingeblendet.
- **Impact-Transparenz:** Bei jedem RFC ist ein Snapshot des Dependency-Graphs zum Zeitpunkt der Planung hinterlegt.
- **Audit-Sicherheit:** Alle Statusübergänge im Change-Lifecycle sind lückenlos im Audit-Log (T1_12) nachvollziehbar.