**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Enterprise Notification Engine (Multi-Channel)  
**ID:** 009  
**Status:** Enterprise-Ready

Aufgabenstellung: Multi-Channel Benachrichtigungssystem

1. Beschreibung

Zentrale Steuerung aller ausgehenden Informationen. Die Engine dient als Schnittstelle zwischen Systemereignissen (Ticketing, Asset-Monitoring, Lifecycle-Events) und den Empfängern. Sie unterstützt Echtzeit-Push (SignalR), E-Mail und perspektivisch Webhooks (Teams/Slack).

2. Funktionsumfang (Enterprise-Standard)

2.1 Multi-Channel Architektur

- **SignalR (Echtzeit):** Sofortige In-App-Benachrichtigung und Windows-Toasts für aktive Agenten.
- **E-Mail (SMTP):** Versand von formatierten HTML-Mails für externe Kontakte, Vorgesetzte oder Offline-Agenten.
- **Persistent Store:** Alle Benachrichtigungen werden in der DB gespeichert (Status: `Sent`, `Delivered`, `Read`), um eine "Inbox-Historie" im User-Portal zu ermöglichen.

2.2 Event-Trigger & Logik (Lifecycle)

Die Engine reagiert auf verschiedene Event-Quellen:

- **Ticketing:** Statusänderungen, neue Kommentare, SLA-Warnungen (Eskalation).
- **Asset-Management:** Ablauf von Garantiefristen (30/60/90 Tage Vorlauf).
- **Security/Certificates:** Ablaufwarnungen für SSL-Zertifikate oder Passwörter (AD-Integration).
- **System-Alerts:** Worker-Ausfälle oder IP-Konflikte im IPAM.

2.3 Empfänger-Matrix (Smart Routing)

Benachrichtigungen werden nicht nur an Personen, sondern an Rollen oder Relationen gesendet:

- **Direkte Zuweisung:** Bearbeiter eines Tickets.
- **Relationen:** Der im AD hinterlegte **Vorgesetzte** (Manager-Attribut) bei kritischen Eskalationen.
- **Owner:** Der primäre Benutzer eines Assets bei anstehendem Hardware-Tausch.
- **Interessentengruppen:** Alle Mitglieder der Gruppe "Netzwerk-Admins" bei Switch-Ausfall.

3. Technische Umsetzung

3.1 Template-Engine

- Nutzung von **Handlebars** oder **Razor-Templates** zur Erstellung konsistenter E-Mails.
- Unterstützung von Platzhaltern (z. B. `{{Ticket.ID}}`, `{{Asset.Name}}`, `{{Expiry.Date}}`).

3.2 Benachrichtigungs-Präferenzen (Opt-out/Opt-in)

- User können im Portal selbst festlegen, über welche Events sie auf welchem Kanal informiert werden möchten (z. B. "Garantie-Ablauf nur per E-Mail", "Ticket-Update per SignalR & Mail").

3.3 Batching & Throttling

- Vermeidung von "Notification Spams" durch Zusammenfassung ähnlicher Events (z. B. "10 Zertifikate laufen bald ab" in einer Mail statt 10 Mails).

4. Workflow

5. **Event-Quelle:** Die Asset-Verwaltung stellt fest: "Garantie für Server X läuft in 30 Tagen ab".
6. **Notification-Job:** Erstellt einen Auftrag für die Engine.
7. **Routing:** Engine prüft: Wer ist der Asset-Owner? Wer ist der zuständige Team-Lead?
8. **Versand:** SignalR-Push an den eingeloggten Admin + E-Mail an den Vorgesetzten.

9. Abnahmekriterien

- **Echtzeit:** SignalR-Meldungen erscheinen in < 1s im WinUI-Client.
- **Zuverlässigkeit:** E-Mails werden bei Nichterreichbarkeit des SMTP-Servers in einer Queue gehalten und wiederholt.
- **Relationales Routing:** Eine Benachrichtigung erreicht korrekt den im AD hinterlegten Vorgesetzten, wenn ein Ticket eskaliert.
- **Lifecycle-Check:** Automatisierte Benachrichtigungen für SSL-Ablauf werden korrekt basierend auf CMDB-Daten ausgelöst.
- **Zentrales Archiv:** User können im "Notification Center" der App alle vergangenen Meldungen einsehen.