**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** ITIL-basiertes Service Management  
**ID:** 004  
**Status:** Enterprise-Refined

Aufgabenstellung: ITSM-Ticketing & SLA-Engine

1. Beschreibung

Implementierung eines ITIL-konformen Service Management Moduls. Das System unterscheidet strikt zwischen verschiedenen Ticket-Typen und nutzt die Daten der CMDB (T2_03) zur Auswirkungsanalyse und Asset-Zuordnung.

2. ITIL-Kernprozesse (Erweiterter Funktionsumfang)

2.1 Incident vs. Service Request Management

- **Incident Management:** Wiederherstellung des Normalbetriebs bei Störungen. Fokus auf **Dringlichkeit** und **Auswirkung**.
- **Service Request:** Standardisierte Anfragen (z.B. "Neuer Laptop", "Passwort-Reset") über einen definierten Service-Katalog.
- **Problem Management (Vorbereitung):** Möglichkeit, mehrere Incidents mit einem "Problem-Ticket" zu verknüpfen (Ursachenforschung).

2.2 CMDB-Integration (Impact-Analyse)

- **Affected CI:** Jedes Ticket muss mit einem oder mehreren **Configuration Items (CIs)** aus der CMDB verknüpft werden können (Server, Software, User).
- **Beziehungs-Mapping:** Bei Auswahl eines CIs zeigt das System automatisch Abhängigkeiten an (z.B. "Wenn dieser Switch gestört ist, sind folgende 20 Server betroffen").

2.3 Enterprise SLA-Engine

- **Service Levels:** Definition von Reaktions- und Lösungszeiten basierend auf dem **Service-Typ** und der **Prioritäts-Matrix** (Dringlichkeit x Auswirkung).
- **Business Hours:** Berücksichtigung von Arbeitszeitkalendern (z.B. 8/5 vs. 24/7) und Feiertagen bei der SLA-Berechnung.
- **Eskalationsmanagement:** Mehrstufige Benachrichtigungen (E-Mail/Teams) vor Erreichen der SLA-Verletzung.

2.4 Kommunikation & Journaling

- **Internes vs. Externes Journal:** Trennung von interner Team-Kommunikation (Private Notes) und Kundennachrichten.
- **E-Mail-Integration:** Erstellung und Beantwortung von Tickets direkt via E-Mail (Mail-to-Ticket-Gateway).

3. Workflow & Automatisierung

4. **Klassifizierung:** Automatische Zuweisung zu Support-Teams (L1, L2, L3) basierend auf Kategorien oder betroffenen CIs.
5. **Status-Maschine:** Strenge Status-Abfolgen (z.B. _New -> In Progress -> Pending Customer -> Resolved -> Closed_).
6. **Wiedereröffnungsschutz:** Verhindert das Reaktivieren von Tickets nach einer definierten "Closed"-Frist.

7. Enterprise-Features

- **Rollenkonzept:** Unterscheidung zwischen _Enduser_ (Portal-Zugriff), _Agent_ (Bearbeiter) und _Manager_ (Reporting/Reporting).
- **Worklogs:** Erfassung der aufgewendeten Zeit pro Ticket für internes Controlling oder Abrechnung.
- **Knowledge Base Link:** Vorschlag von FAQ-Artikeln basierend auf dem Ticket-Inhalt (KI-unterstützt).

5. Abnahmekriterien (Enterprise-Ready)

- **CI-Verknüpfung:** Ein Incident zeigt alle verknüpften Assets aus der CMDB an.
- **SLA-Genauigkeit:** Die Restzeit berechnet sich korrekt unter Ausschluss von Wochenenden/Nachtzeiten.
- **Audit-Sicherheit:** Jede Feldänderung (Status, Priorität, Zuweisung) wird manipulationssicher im Audit-Log festgehalten.
- **Mandantenfähigkeit:** Tickets können verschiedenen Organisationseinheiten oder Kunden zugeordnet werden, ohne dass diese gegenseitig Daten einsehen können