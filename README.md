

## InfraDesk - Projekt-Masterplan
### 1. Projektdefinition

InfraDesk ist eine modulare Enterprise-IT-Management-Lösung. Das Ziel ist es, isolierte Datensilos (CMDB, IPAM, Ticketing, Dokumentation) in einer einzigen, performanten Plattform zu vereinen.

#### Kernversprechen

- Single Source of Truth: Alle IT-Assets und deren Abhängigkeiten an einem Ort.
- Automatisierung: Agentenlose Erfassung und sicheres Credential-Management mit Anbindung an externe Passwort-Manager.
- Maximale Datenportabilität: Universal-Export (CSV) in allen Tabellenansichten und hocheffiziente Massen-Import-Tools.
- Inventar-Sicherheit: Eindeutige Identifizierung via QR/Barcode und mobile Status-Updates.
- Enterprise Ready: Mandantenfähigkeit, Hochverfügbarkeit und Auto-Update-Logik.

### 2. Technologische Strategie

- Architektur: Clean Architecture (Core, Application, Infrastructure, UI/API).
- Frontend: WinUI 3; Auto-Update-Komponente für Client-Verteilung.
- Backend: .NET 8 Web-API.
- Datenhaltung: PostgreSQL 16 mit JSONB; AES-256 Tresor für Passwörter.
- Echtzeit: SignalR für sofortige Updates im Client.
- Mobile: Native Apps mit Kamera-Integration.
- Integrationen: Schnittstellen zu Authentifizierungsdiensten und Passwort-Managern (Bitwarden/Vaultwarden, KeePass/XC, 1Password).

# Repository-Struktur

InfraDesk/
├── .github/                   # GitHub-spezifische Konfigurationen
├── docs/                      # Die gesamte Planung & Dokumentation (Stufe 0-4)
├── src/                       # Quellcode (Clean Architecture & MVVM)
│   ├── InfraDesk.Core/        # [Models] Domänenentitäten
│   ├── InfraDesk.Application/ # Use Cases & Interfaces
│   ├── InfraDesk.Infrastructure/ # Datenzugriff (EF Core)
│   ├── InfraDesk.API/         # ASP.NET Core Web-API
│   ├── InfraDesk.UI.WinUI/    # [MVVM Client] WinUI 3 App
│   └── InfraDesk.sln          # Visual Studio Solution
├── tests/                     # Unit- & Integrationstests
├── scripts/                   # Inno Setup & PowerShell Scripte
├── .gitignore                 # Visual Studio & WinUI spezifisch
└── README.md                  # Projekt-Einstiegspunkt

# InfraDesk - Dokumentations-Index (Master-Backlog)

Dieser Index listet alle Dateien auf, die im Rahmen der Projektierung von InfraDesk erstellt wurden. Die Struktur folgt der im PowerShell-Skript definierten Ordnerhierarchie.

📁 00_Strategie - Zentrale Dokumente, die die Vision und die logischen Leitplanken definieren.

- 000_Masterplan.md: Projektdefinition, Technologiestack und Roadmap.

🏗️ 01_Stufe_0: Vorbereitung - Grundlagen für die Entwicklung und Verteilung.

- T0_01_Architecture_Setup.md: Clean Architecture & MVVM Projektstruktur.
- T0_13_Deployment_InnoSetup.md: Erstellung der Setup.exe.
- T0_14_AutoUpdate_Mechanism.md: Client-seitige Update-Logik.

🧱 02_Stufe_1: Fundament - Das funktionale MVP (Minimum Viable Product).

- T1_01_Kickoff.md: Start der Fundament-Phase.
- T1_02_Database_PostgreSQL.md: DB-Design & Mandantentrennung.
- T1_03_CMDB_Dynamic_Model.md: Core-CMDB & Asset-Links.
- T1_04_Ticketing_Foundation.md: Basis-Ticketing & SLAs.
- T1_05_IPAM_Basics.md: IP-Verwaltung & DHCP-Monitoring.
- T1_06_Identity_Access.md: IAM & Entra ID Integration.
- T1_07_Discovery_Worker_Setup.md: Hintergrunddienst-Architektur.
- T1_08_JSONB_Validation.md: Validierung dynamischer Felder.
- T1_09_Notification_Engine.md: SignalR Echtzeit-Push & E-Mail.
- T1_10_Attachment_Management.md: Dateispeicherung in PostgreSQL.
- T1_11_Search_FullText.md: Modulübergreifende Volltextsuche.
- T1_12_Audit_Logging.md: Revisionssicheres Log-System.
- T1_14_Backup_Recovery.md: Datensicherung & Restore.
- T1_15_Dashboards_Reports.md: Visualisierung & KPIs.
- T1_16_SelfService_Portal.md: Web-Oberfläche für Anwender.
- T1_17_Accessibility_UI.md: WCAG-Konformität.
- T1_18_API_SDK_Docs.md: OpenAPI & PowerShell SDK.
- T1_19_Asset_Labeling.md: Inventarnummern & QR-Code-Druck.
- T1_20_Data_Migration.md: CSV/Excel Massen-Import.
- T1_21_Credential_Vault.md: Bitwarden/KeePass/1Password Sync.
- T1_22_Global_CSV_Export.md: Export-Funktion für alle Tabellen.

🚀 03_Stufe_2: Automatisierung - Intelligenz und tiefgreifende ITIL-Prozesse.

- T2_01_Kickoff.md: Start der Automatisierungs-Phase.
- T2_02_Dependency_Visuals.md: Interaktive Graphen & Impact-Analyse.
- T2_03_Infrastructure_Scan.md: WMI/AD/DNS Tiefenanalyse.
- T2_04_License_Management.md: Software Asset Management (SAM).
- T2_05_ITSM_Processes.md: Change- & Problem-Management.
- T2_06_Database_HA.md: PostgreSQL Hochverfügbarkeit (Patroni).
- T2_07_Contract_Management.md: Vertragsverwaltung & Laufzeiten.
- T2_08_Disaster_Recovery.md: DR-Pläne & Wiederherstellungslogik.
- T2_09_Inventory_Diff.md: Snapshot-Vergleich & Historie.

✨ 04_Stufe_3: Mehrwert - Prozesse, Mobile Apps und Integration.

- T3_01_Kickoff.md: Internationalisierung & Workflows.
- T3_02_Workflow_Designer.md: Grafische Prozessmodellierung.
- T3_03_Mobile_Scan.md: Smartphone-Inventur & Quick-Actions.
- T3_04_Geo_Visuals.md: Karten & Etagenpläne.
- T3_05_Service_Catalog.md: Bestellwesen & Katalog.
- T3_06_Chat_Integration.md: Teams/Slack Bot & Webhooks.
- T3_07_Knowledge_Base.md: Wissensdatenbank & FAQ.

🌐 05_Stufe_4: Expansion - Enterprise-Integration und Spezialthemen.

- T4_01_Kickoff.md: Start der Expansion-Phase.
- T4_02_Monitoring.md: PRTG/Zabbix Live-Status.
- T4_03_Offline_Mode.md: SQLite-Sync & Resilienz.
- T4_04_DLP.md: Data Loss Prevention.
- T4_05_Federation.md: ServiceNow/Jira Abgleich.
- T4_06_Energy.md: Green-IT & Energieverbrauch.
- T4_07_Plugins.md: Erweiterbarkeit durch Plugins.
- T4_08_Scaling.md: Docker & Kubernetes Skalierung.
- T4_09_Diagnostics_Licensing.md: Support-Tools & App-Lizenzierung.