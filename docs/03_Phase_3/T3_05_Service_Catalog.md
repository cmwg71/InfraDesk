**Phase:** Stufe 3 (Mehrwertfunktionen)  
**Aufgabe:** Servicekatalog & Self-Service (ITSM-Standard)  
**ID:** 035  
**Status:** Enterprise-Ready

Aufgabenstellung: Servicekatalog & Request Fulfillment

1. Beschreibung

Implementierung eines zentralen **Service-Katalogs** als einzige Schnittstelle für Business-Anforderungen. Der Prozess folgt dem ITIL-Framework für _Request Fulfillment_. Jeder Service ist an hinterlegte Kosten, SLAs und automatisierte Workflows gebunden.

2. Funktionsumfang (Enterprise-Standard)

2.1 Service-Definition & Klassifizierung

- **Service-Karten:** Strukturierte Darstellung mit Icons, Kurzbeschreibung, geschätzter Bereitstellungszeit und Kostenstellen-Relevanz.
- **SLA-Koppelung (ITIL-konform):**
    - **OLA (Operational Level Agreement):** Interne Zeitziele für die IT-Teams.
    - **UC (Underpinning Contract):** Berücksichtigung von Lieferzeiten externer Partner.
- **Sichtbarkeit (Entitlements):** Steuerung, welcher User welche Services sieht (z. B. "Entwickler-Tools" nur für die Abteilung IT).

2.2 Dynamische Request-Formulare

- **Form-Builder:** Drag-and-Drop Editor für Service-spezifische Felder (Dropdowns, Checkboxen, File-Uploads).
- **Validierung:** Echtzeit-Prüfung gegen die CMDB (z. B. bei "Zusatz-RAM": Prüfung, ob das aktuelle Gerät des Users überhaupt aufrüstbar ist).

2.3 Multi-Stage Genehmigungs-Workflows

- **Hierarchische Approvals:** Automatische Ermittlung des Vorgesetzten via AD-Manager-Attribut (Modul 023).
- **Financial Approval:** Zusätzliche Genehmigung durch Kostenstellenverantwortliche bei Überschreitung von Budget-Grenzwerten.
- **Status-Tracking:** Transparente Anzeige des Fortschritts für den Endanwender ("Wartet auf Genehmigung", "In Beschaffung", "Versandbereit").

2.4 Lifecycle-Automatisierung (Provisioning)

- **Asset-Integration:** Nach Abschluss eines Hardware-Requests (z. B. Monitor) wird das Asset in der CMDB automatisch auf `In Use` gesetzt und mit dem Besteller verknüpft.
- **Software-Provisioning:** Schnittstelle zu AD/Entra ID, um User nach Genehmigung automatisch in die entsprechende Berechtigungsgruppe (Modul 023) aufzunehmen.

3. Technische Umsetzung

- **Workflow-Engine:** Anbindung an das Modul T3_02 zur Steuerung der logischen Abfolge (Genehmigung -> Ticket-Erstellung -> Asset-Zuweisung).
- **Self-Service Portal (PWA):** Mobile-optimierte Oberfläche für Benutzer, um Anfragen auch von unterwegs zu genehmigen oder zu erstellen.

4. Enterprise-Governance

- **Service-Level-Reporting:** Dashboards über die Einhaltung der versprochenen Lieferzeiten pro Service-Kategorie.
- **Audit-Trail:** Lückenlose Dokumentation, wer wann eine Anforderung genehmigt oder abgelehnt hat (Modul 012).

5. Abnahmekriterien

- **End-to-End Prozess:** Ein User bestellt einen Service -> Vorgesetzter erhält Benachrichtigung (T1_09) -> Nach Genehmigung entsteht automatisch ein Task für die IT.
- **SLA-Validierung:** Die berechnete "Target Resolution Time" im Ticket berücksichtigt die im Service-Katalog definierte SLA-Zeit.
- **CMDB-Consistency:** Nach Auslieferung eines physischen Assets ist die Beziehung zwischen `User` und `Asset` ohne manuellen Eingriff korrekt hinterlegt.
- **RBAC-Check:** Ein Standard-User kann keine "Management-Services" (z. B. Server-Bestellung) sehen oder anfordern.