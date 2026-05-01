# InfraDesk - Enterprise CMDB & IT Management Solution

InfraDesk ist eine modulare, webbasierte IT-Management-Plattform, die darauf spezialisiert ist, isolierte Datensilos in einer zentralen, performanten Lösung zu vereinen. Ursprünglich als Desktop-Anwendung konzipiert, nutzt InfraDesk heute die modernsten Web-Technologien auf Basis von .NET 8 Blazor, um plattformunabhängige Flexibilität mit Enterprise-Power zu kombinieren.

## 🚀 Kernfunktionen

### 🖥️ Asset-CMDB (Configuration Management Database)

Single Source of Truth: Alle Hardware-Assets und deren Abhängigkeiten an einem zentralen Ort.

Dynamisches Datenmodell: Speicherung flexibler Attribute via PostgreSQL JSONB.

Lebenszyklus-Management: Vollständig konfigurierbare Phasen von "Planung & Design" bis "Außerbetriebnahme".

Inventar-Sicherheit: Eindeutige Identifizierung über systemgenerierte Inventarnummern und QR-Code-Integration.

### 🌐 IPAM & Netzwerk-Management

Relationales IP-Modell: Jede IP ist ein Objekt, keine bloße Text-Eigenschaft.

DHCP-Monitoring: Überwachung von Reservierungen und Abgleich mit der CMDB (Discovery-Worker Integration).

Subnetz-Hierarchie: Verwaltung von Netzen, Gateways und VLAN-Strukturen.

### 🎫 Ticketing & Helpdesk (Foundation)

Incident-Management: Erfassung und Bearbeitung von IT-Störungen.

Asset-Kopplung: Direkte Verknüpfung von Tickets mit betroffenen Hardware-Komponenten.

Journal: Lückenlose Historie der Kommunikation und Statusänderungen.

### 🎨 Design & Personalisierung

Harmonische Paletten: 10 vordefinierte Design-Konzepte (z.B. Midnight Deep Ocean, Nature Inspired, Corporate Blue).

High Contrast Mode: Barrierefreiheit nach WCAG-Standards.

Live-Theme-Engine: Sofortige Anwendung von Farb- und Typografie-Änderungen im gesamten System.

## 🏗️ Architektur (Clean Architecture)

```text
InfraDesk folgt strikt dem Schichtenmodell, um Wartbarkeit und Testbarkeit zu garantieren:
 └── 
    ├── InfraDesk.Core: Domänen-Entitäten (Asset, Ticket, IpAddress) und Kernlogik.
    |
    ├── InfraDesk.Application: Use Cases, Interfaces und Geschäftslogik.
    |
    ├── InfraDesk.Infrastructure: Datenzugriff via Entity Framework Core, PostgreSQL-Konfiguration und externe Services.
    |
    ├── InfraDesk.UI.Web: Das moderne Web-Frontend (Blazor Interactive Server) mit MudBlazor-Komponenten.
    |
    ├── InfraDesk.API: Zentrale REST-Schnittstelle für den Discovery-Worker und externe Integrationen.
    |
    └── InfraDesk.Worker: Hintergrunddienst zur agentlosen Erfassung (WMI, DNS, Ping).
```text

### 🛠️ Technologiestack

Backend: .NET 8 (ASP.NET Core Web API)

Frontend: Blazor Web App (Interactive Server Mode)

UI-Bibliothek: MudBlazor (Material Design)

Datenbank: PostgreSQL 16 (mit Fokus auf JSONB Performance)

ORM: Entity Framework Core

Update-Checker: Integration der GitHub REST API (inkl. Support für private Repos via PAT)

## 🏁 Erste Schritte (Entwicklung)

### Voraussetzungen

.NET 8 SDK

PostgreSQL 16 Instanz

Visual Studio 2022 (v17.8+) oder VS Code

### Installation

Repository klonen:

git clone [https://github.com/cmwg71/InfraDesk.git](https://github.com/cmwg71/InfraDesk.git)

Datenbank-Verbindung in src/InfraDesk.API/appsettings.json anpassen.

Migrationen ausführen:

dotnet ef database update --project src/InfraDesk.Infrastructure --startup-project src/InfraDesk.API

Projekte starten (Multi-Project Startup): InfraDesk.API und InfraDesk.UI.Web.

## 📈 Versionierung & Updates

Die Versionierung folgt dem Enterprise-Format: YYYY.MM.TT.Buildnumber.
Die aktuelle Version ist im Info-Bereich der Applikation einsehbar und kann live gegen das GitHub-Repository auf Aktualität geprüft werden.

# ✒️ Autor & Copyright

Christian Grams
Inhaber von Grams IT

Entwickelt für professionelle IT-Infrastrukturen und hochverfügbares Asset-Management.

© 2026 Grams IT. Alle Rechte vorbehalten.