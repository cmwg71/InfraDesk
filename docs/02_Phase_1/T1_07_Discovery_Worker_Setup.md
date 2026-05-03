**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Architektur des Discovery-Workers  
**ID:** 007  
**Status:** Enterprise-Ready

Aufgabenstellung: Discovery-Worker (Multi-Protocol Orchestrator)

1. Beschreibung

Der Discovery-Worker ist ein dezentraler Hintergrunddienst, der die aktive Kommunikation mit der Infrastruktur übernimmt. Er agiert als Proxy: Er erhält abstrakte Scan-Aufträge vom Backend und übersetzt diese in protokollspezifische Abfragen (CIM, SNMP, DHCP-API).

2. Erweiterte Komponenten & Logik

2.1 Worker-Dienst (.NET 8 Windows Service)

- **Modularer Aufbau:** Plugin-Architektur für Scan-Module (CIM-Modul, SNMP-Modul, DHCP-Modul).
- **Concurrency Management:** Einstellbare Anzahl paralleler Scans, um die Netzlast zu kontrollieren.
- **Local Secret Store:** Temporäres Caching von Credentials aus dem Backend-Vault (nur im RAM).

2.2 Kommunikations-Design (Pull-Prinzip)

- **Heartbeat:** Worker meldet sich alle 30s beim Backend (Status: Idle, Busy, Error).
- **Job-Queue:** Abruf von Aufgaben via REST-API oder SignalR für Echtzeit-Trigger.
- **Payload-Sicherheit:** AES-256 Verschlüsselung der Ergebnisse vor dem Rückversand via TLS 1.3.

2.3 DHCP-Überwachungs-Modul (Neu)

- **Native Integration:** Nutzung der `Microsoft.Management.Infrastructure` (CIM) für Windows DHCP-Server.
- **Monitoring-Logik:**
    - Regelmäßiger Abgleich der `Get-DhcpServerv4Reservation`.
    - Überwachung der Scope-Füllstände (Utilization).
    - _Enterprise-Feature:_ Erkennung von "Unauthorized DHCP Servers" im Subnetz (Rogue DHCP Detection).

3. Scan-Phasen & Protokoll-Stack

Der Worker arbeitet nach einem festen Entscheidungsbaum (Protocol-Ladder):

1. **Phase 1: Discovery (L2/L3)**
    - ICMP Ping & ARP-Check zur Präsenzerkennung.
    - DNS-Reverse-Lookup zur Identifikation des FQDN.
2. **Phase 2: Fingerprinting (L4/L7)**
    - Port-Scan auf Schlüssel-Ports: 5985/5986 (WinRM), 161 (SNMP), 443 (Web/API).
3. **Phase 3: Deep Scan (Auth-basiert)**
    - **Windows:** Abfrage via **CIM/PowerShell** (Hardware, Software, AD-Relationen).
    - **Network:** Abfrage via **SNMP v2c/v3** (Netbox-Metadaten).
    - **Infrastructure:** Abfrage von DHCP-Ressourcen.

4. Sicherheits- & Berechtigungskonzept

- **Service-Account:** Betrieb unter einem "Managed Service Account" (gMSA) für automatische Passwort-Rotation.
- **Just-In-Time (JIT) Scanning:** Integration in PAM-Systeme (Privileged Access Management) möglich, bei dem der Worker Credentials nur für die Dauer des Scans erhält.
- **Network Scoping:** Der Worker akzeptiert nur Job-Definitionen für IP-Bereiche, die explizit in seiner lokalen Konfiguration (Whitelist) erlaubt sind.

5. Abnahmekriterien

- **Connectivity:** Worker baut eine persistente, verschlüsselte Verbindung zum API-Gateway auf.
- **Multi-Threading:** Worker verarbeitet mindestens 50 IP-Adressen simultan (einstellbar).
- **DHCP-Read:** Erfolgreiches Auslesen von DHCP-Reservierungen eines Windows-Servers ohne Installation lokaler Agenten.
- **CIM-Fallback:** Korrekte Erkennung, wenn WinRM deaktiviert ist und saubere Fehlermeldung an das Staging-System.
- **Efficiency:** Delta-Reporting (nur Änderungen werden gesendet, um Bandbreite zu sparen).