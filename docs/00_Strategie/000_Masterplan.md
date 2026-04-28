## Dateiname: 000_Masterplan.md

Phase: Übergreifend Aufgabe: Projektdefinition & Vorgehensmodell Beschreibung: Zentrales Dokument zur Definition von InfraDesk und der Entwicklungs-Roadmap. ID: 000

# InfraDesk - Projekt-Masterplan

## 1. Projektdefinition

InfraDesk ist eine modulare Enterprise-IT-Management-Lösung. Das Ziel ist es, isolierte Datensilos (CMDB, IPAM, Ticketing, Dokumentation) in einer einzigen, performanten Plattform zu vereinen.

### Kernversprechen

- **Single Source of Truth**: Alle IT-Assets und deren Abhängigkeiten an einem Ort.
- **Automatisierung**: Agentenlose Erfassung und **sicheres Credential-Management** mit Anbindung an externe Passwort-Manager.
- **Maximale Datenportabilität**: Universal-Export (CSV) in allen Tabellenansichten und hocheffiziente Massen-Import-Tools.
- **Inventar-Sicherheit**: Eindeutige Identifizierung via QR/Barcode und mobile Status-Updates.
- **Enterprise Ready**: Mandantenfähigkeit, Hochverfügbarkeit und **Auto-Update-Logik**.
## 2. Technologische Strategie

- **Architektur**: Clean Architecture (Core, Application, Infrastructure, UI/API).
- **Frontend**: WinUI 3; **Auto-Update-Komponente** für Client-Verteilung.
- **Backend**: .NET 8 Web-API.
- **Datenhaltung**: PostgreSQL 16 mit JSONB; **AES-256 Tresor** für Passwörter.
- **Echtzeit**: SignalR für sofortige Updates im Client.
- **Mobile**: Native Apps mit Kamera-Integration.
- **Integrationen**: Schnittstellen zu Authentifizierungsdiensten und Passwort-Managern (**Bitwarden/Vaultwarden, KeePass/XC, 1Password**).

## 3. Vorgehensmodell

Die Entwicklung erfolgt in einem iterativen Phasenmodell (Stufen):

| **Stufe** | **Fokus**           | **Ziel**                                                                   |
| --------- | ------------------- | -------------------------------------------------------------------------- |
| **0**     | **Vorbereitung**    | Architektur, Inno Setup, Auto-Update-Infrastruktur.                        |
| **1**     | **Fundament**       | CMDB, **Massen-Import-Assistent**, Inventar-ID, IPAM, **Credential-Sync**. |
| **2**     | **Automatisierung** | Discovery-Worker (Deep Scan), ITSM-Prozesse, HA.                           |
| **3**     | **Mehrwert**        | Workflows, Mobile Scan, Geo-Visualisierung.                                |
| **4**     | **Expansion**       | Offline-Modus, DLP, Monitoring-Integrationen.                              |

## 4. Detaillierte Vorgehensweise (Workflow)

1. **Analyse & Schema**: DB-Erweiterungen (z.B. verschlüsselte Felder, Import-Mapping).
2. **Logic & Validation**: Business-Logik, Kryptographie & **CSV-Engine** im Application-Layer.
3. **API-Contract**: Endpunkte für Migration, Sync-Schnittstellen und Updates.
4. **UI-Implementation**: WinUI für Import-Wizards, globale Export-Buttons und Einstellungs-Dialoge.
5. **Testing**: Fokus auf Verschlüsselungstests, Import-Validierung und API-Konnektivität zu Drittanbietern.
## 5. Leitplanken für die Entwicklung

1. **API-First**: Jede Funktion ist via API steuerbar.
2. **Security by Design**: Secrets niemals im Klartext, strikte Mandantentrennung.
3. **Daten-Souveränität**: Nutzer können ihre Daten jederzeit exportieren (CSV/Excel).
4. **Interoperabilität**: Offenheit für externe Passwort-Manager und Auth-Provider.
5. **Traceability**: Importe, Exporte und Updates werden lückenlos auditiert.