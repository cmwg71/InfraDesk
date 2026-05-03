**Phase:** Stufe 2 (Erweiterte Kernfunktionen)  
**Aufgabe:** Enterprise Software Asset Management (SAM)  
**ID:** 024  
**Status:** Enterprise-Ready

Aufgabenstellung: SAM & Compliance-Steuerung

1. Beschreibung

Implementierung eines zentralen Moduls zur Verwaltung des Software-Lifecycles. Das Ziel ist die Herstellung einer ständigen Audit-Bereitschaft durch den automatisierten Abgleich von Lizenzansprüchen (Entitlements) mit der tatsächlichen Nutzung (Consumption).

2. Funktionsumfang (Enterprise-Standard)

2.1 Erweiterte Lizenz-Modelle

- **Metriken:** Unterstützung verschiedener Lizenztypen (Per User, Per Device, Per Core/CPU, SaaS-Subskriptionen).
- **Vertragsbindung:** Hinterlegung von Laufzeiten, Kündigungsfristen und Wartungsverträgen (Maintenance/Support) inkl. Dokumenten-Upload (Modul T1_10).
- **Pool-Management:** Verwaltung von Volumenlizenzen (z. B. Microsoft M365 oder Adobe CC), bei denen ein Kontingent flexibel zugewiesen wird.

2.2 Automatisierter Compliance-Engine

- **Discovery-Abgleich:** Die Engine korreliert die Scan-Daten des Workers (T2_03) mit den Lizenz-Pools.
- **Normalisierung:** Umwandlung verschiedener Software-Schreibweisen (z. B. "Acrobat Reader 2024" vs. "Adobe Acrobat Rdr") in ein einheitliches Software-Produkt mittels Alias-Tabellen.
- **Überschuss-Analyse:** Identifikation von ungenutzten, aber lizenzierten Produkten (Potenzial zur Kosteneinsparung).

2.3 Warn- & Eskalationswesen

- **Mismatch-Indikator:** Dynamisches Dashboard (Rot/Gelb/Grün) für die Compliance-Rate pro Software-Hersteller.
- **Proaktive Benachrichtigung (T1_09):** Automatische Warnung an den Lizenzmanager, wenn:
    - Die Auslastung 90% erreicht.
    - Ein Lizenzvertrag oder Support in 60 Tagen abläuft.
    - Unerlaubte Software (Blacklist) vom Worker gefunden wurde.

3. Technische Umsetzung

3.1 Relationales Mapping

- **n:m Beziehungen:** Eine Lizenz kann mehreren Assets zugeordnet sein; ein Asset kann Dutzende Lizenzen (OS, Office, Fachanwendungen) tragen.
- **Zuweisungs-Logik:** Unterscheidung zwischen "Hard-Zuweisung" (manuell fixiert) und "Auto-Zuweisung" (basierend auf Scan-Regeln).

3.2 Audit-Trail (Modul 012)

- Revisionssichere Protokollierung jeder Lizenzübertragung und jeder Änderung am Lizenzbestand für externe Wirtschaftsprüfer.

4. Workflow (Software-Audit)

5. **Input:** Einkauf hinterlegt 100 Lizenzen für "Produkt X".
6. **Scan:** Worker meldet 115 Installationen im Netzwerk.
7. **Alert:** System erzeugt sofort einen "Compliance-Incident" und markiert die 15 überzähligen Assets.
8. **Action:** Lizenzmanager entscheidet: Nachkauf oder Deinstallation (Workflow-Trigger).

9. Abnahmekriterien

- **Compliance-Dashboard:** Zentrale Ansicht aller kritischen Abweichungen in Echtzeit.
- **Drill-Down:** Klick auf eine Lizenz-Warnung zeigt exakt die betroffenen Assets an.
- **Vertrags-Reminder:** Benachrichtigungs-Engine löst korrekt bei Ablaufdatum aus.
- **Software-Katalog:** Möglichkeit zur Definition einer "White-List" (erlaubte Standardsoftware).