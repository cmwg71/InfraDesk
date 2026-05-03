**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Revisionssicheres Attachment Management  
**ID:** 010  
**Status:** Enterprise-Ready (Ready for Development)

Aufgabenstellung: DB-basierte Dateiverwaltung (Binary Large Objects)

1. Beschreibung

Zentraler Dienst zur Verwaltung aller Dateianhänge innerhalb der Anwendung. Durch die Speicherung in PostgreSQL (`bytea`) wird eine lückenlose Revisionssicherheit (WORM-Prinzip) garantiert: Ein Datenbank-Backup enthält alle Dokumente im exakten Zustand zum Zeitpunkt der Sicherung, untrennbar verknüpft mit den Audit-Logs.

2. Technisches Design & Storage-Strategie

2.1 Tabellen-Partitionierung (Performance-Isolation)

Um "Table Bloat" zu verhindern und die Performance beim Browsen von Listen zu erhalten, werden Metadaten und Binärdaten strikt getrennt:

- **Tabelle `Attachments` (Metadaten):**
    - ID, Dateiname, Mime-Type, Größe.
    - SHA256-Hash (zur Integritätsprüfung und Deduplizierung).
    - Zuordnung: `AssetID`, `TicketID` oder `UserID`.
    - Status: `Pending_Scan`, `Clean`, `Infected`.
- **Tabelle `AttachmentContent` (Binärdaten):**
    - `AttachmentID` (FK), `Data` (bytea - Originaldatei).
    - `ThumbnailData` (bytea - Vorschau-Objekt).

2.2 Streaming & RAM-Management

- **Stream-Only-Processing:** Die API puffert Dateien nicht im Arbeitsspeicher, sondern streamt Daten direkt zwischen Client und PostgreSQL.
- **Chunked Upload:** Unterstützung für fragmentierte Uploads bei großen Dateien oder instabilen Verbindungen.

3. Enterprise-Features

3.1 Revisionssicherheit (WORM & Versioning)

- **Immutability:** Einmal gespeicherte Blobs können nicht editiert werden. Jede Änderung erzeugt ein neues Attachment-Objekt inklusive Audit-Eintrag in Modul 012.
- **Integrität:** Bei jedem Download wird der SHA256-Hash validiert, um sicherzustellen, dass die Datei seit dem Upload nicht verändert wurde.

3.2 Thumbnail-Service (Performance-Layer)

- **Asynchrone Generierung:** Nach dem Upload erstellt ein Hintergrund-Job automatisch Thumbnails für Bildformate (JPG, PNG, WebP) und optional die erste Seite von PDFs.
- **Lazy Loading:** Die Web-UI lädt in Galerien oder Journalen primär die `ThumbnailData`. Das Original-Blob wird erst beim expliziten Download/Klick angefordert.

3.3 Sicherheit & Malware-Scan

- **Viren-Scan:** Integration eines Virenscanners (z. B. ClamAV). Dateien sind im Status `Pending_Scan` für reguläre User gesperrt.
- **RBAC-Bindung:** Der Zugriff auf die Datei-ID erfolgt ausschließlich, wenn der User Berechtigungen auf das übergeordnete CI (Asset/Ticket) besitzt.

4. Workflow

5. **Upload:** Client sendet Stream an API -> API berechnet Hash und schreibt in `Attachments`.
6. **Processing:** Hintergrunddienst prüft auf Viren und generiert das Thumbnail.
7. **Verknüpfung:** Das Attachment wird im Audit-Log des CIs vermerkt.
8. **Abruf:** UI zeigt Thumbnail -> Klick triggert Stream-Download des Originals.

9. Abnahmekriterien

- **Backup-Integrität:** Ein SQL-Dump stellt die vollständige Umgebung inkl. aller Dokumente wieder her.
- **Performance:** Listenansichten bleiben trotz vieler Anhänge performant (kein Laden der Blobs in Listen).
- **Revisionssicherheit:** Gelöschte Anhänge werden als "Deleted" markiert, verbleiben aber im Audit-Kontext (Storage-Policy abhängig).
- **Limit-Enforcement:** Upload-Limits (Standard 20MB) werden serverseitig strikt erzwungen.
- **Validierung:** Nachweislich korrekte Generierung von Thumbnails innerhalb von < 5 Sekunden nach Upload.