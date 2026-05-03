**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Inventar-Identifikation & Label-Druck  
**ID:** 019  
**Status:** Enterprise-Ready

Aufgabenstellung: Physische Identifikation & Enterprise-Labeling

1. Beschreibung

Implementierung eines Systems zur eindeutigen Kennzeichnung physischer Assets. Dies umfasst die Logik zur Generierung von Inventarnummern, die visuelle Aufbereitung (QR/Barcode) und die Ansteuerung industrieller Drucksysteme.

2. Funktionsumfang (Enterprise-Standard)

2.1 Intelligente Inventarnummern (Asset-Tagging)

- **Namensschema-Konfigurator:** Zentral steuerbare Logik für IDs (z.B. `[PREFIX]-[JAHR]-[COUNTER]`).
- **Immutable IDs:** Einmal vergebene Inventarnummern sind unveränderlich und werden bei Löschung eines Assets nicht wiederverwendet (Audit-Sicherheit).
- **Validierung:** Prüfung auf Dubletten bereits auf Datenbank-Ebene (Unique Constraint).

2.2 Dynamische QR-Code Logik

- **Deep-Links:** Der QR-Code enthält eine signierte URL, die direkt zur Mobil-Ansicht des Assets im Self-Service Portal führt.
- **Offline-Information:** Optionaler Einbau von Text-Informationen (Hostname, Support-Hotline) direkt in das Label-Design für den Fall, dass kein Netzwerk verfügbar ist.
- **Security-Hash:** Einbau einer kurzen Prüfsumme in die URL, um das "Raten" von Asset-URLs durch Scannen aufeinanderfolgender Nummern zu erschweren.

2.3 Professionelles Label-Management

- **Template-Engine:** Unterstützung für **ZPL (Zebra Programming Language)** und **Brother P-Touch** Formate.
- **Multi-Layout:**
    - _Small:_ Nur QR-Code + ID (für Kleinstgeräte).
    - _Standard:_ QR-Code, ID, Hostname, Kostenstelle.
    - _Server-Tag:_ Inklusive Rack-Position und Service-Tag des Herstellers.
- **Bulk-Printing:** Warteschlange (Print Queue) für Massendruck-Aufträge (z.B. bei Neueinzug von 50 Laptops).

3. Workflow & Integration

4. **Erfassung:** Asset wird manuell angelegt oder per Discovery-Worker (T2_03) erkannt.
5. **ID-Zuweisung:** System generiert die nächste freie Inventarnummer basierend auf der Kostenstelle/Standort.
6. **Druck:** Mitarbeiter wählt im WinUI-Client ein oder mehrere Assets aus und sendet diese an einen definierten Label-Drucker.
7. **Verknüpfung:** Der Status des Assets wechselt automatisch auf "Labeled" oder "In Use".

8. Enterprise-Sicherheit & Governance

- **Verantwortlichkeits-Pflicht:** Ein Druck ist erst möglich, wenn ein "Besitzer" (AD-User) oder eine "Kostenstelle" (Finanz-Modul) zugewiesen wurde.
- **Revisionssicherheit:** Im Audit-Log (Modul 012) wird vermerkt, wer wann ein Etikett für das Asset gedruckt hat (Schutz vor Inventar-Manipulation).

5. Abnahmekriterien

- **Eindeutigkeit:** Die Inventarnummer ist global über alle Mandanten (Tenants) hinweg eindeutig.
- **Mobile-Readiness:** Ein Scan des QR-Codes mit einem Smartphone öffnet direkt die korrekte Asset-Seite (nach Login).
- **Template-Flexibilität:** Wechsel zwischen verschiedenen Etikettengrößen ohne Code-Anpassung (via Konfiguration).
- **Drucker-Kompatibilität:** Erfolgreicher Testdruck auf mindestens einem Industrie-Standard-Drucker (z.B. Zebra).
- **Massendruck:** 100 Labels können in einem Durchgang ohne Performance-Einbußen an die Druckwarteschlange übergeben werden.