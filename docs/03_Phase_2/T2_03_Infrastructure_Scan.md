**Phase:** Stufe 2 (Erweiterte Kernfunktionen)  
**Aufgabe:** Automatisierte Infrastruktur-Dokumentation  
**ID:** 023  
**Status:** Entwicklungsbereit

Aufgabenstellung: Infrastruktur-Tiefenanalyse (CIM, SNMP & AD-Context)

1. Beschreibung

Implementierung einer agentenlosen Tiefenanalyse zur automatisierten Befüllung der CMDB. Das Modul ersetzt manuelle Pflege durch regelmäßige Scans via **Discovery-Worker (T1_07)**. Fokus liegt auf der Abbildung technischer Details sowie logischer Beziehungen (Berechtigungen, Strukturen).

---

2. Scan-Module & Protokolle

2.1 Windows-Systeme (Native PowerShell / CIM)

_WMI wird aufgrund der Abkündigung durch Microsoft durch CIM (Common Information Model) via WinRM ersetzt._

- **Protokoll:** WinRM (Port 5985 HTTP / 5986 HTTPS).
- **Hardware & OS:** Modell, Seriennummer, CPU-Kerne, RAM-Layout, OS-Build & Edition.
- **Storage:** Logische/Physische Disks, Partitionslayout, BitLocker-Status (Verschlüsselungsgrad).
- **Services & Software:** Alle installierten Programme (MSI/Appx), Windows-Updates (KB-Nummern) und Dienst-Status.

2.2 Active Directory Deep-Scan (Identitäten & Governance)

_Abfrage via AD-PowerShell-Modul zur Erfassung der logischen Infrastruktur._

- **OU-Struktur:** Vollständiger Import des OU-Baums inkl. `DistinguishedName` (DN) zur hierarchischen Asset-Navigation.
- **User & Gruppen:**
    - Benutzer-Stammdaten (Mail, Abteilung, Manager-Attribut für Organigramme).
    - Mitgliedschaften (Auflösung verschachtelter Gruppen) zur Erstellung einer **Berechtigungsmatrix**.
    - Identifikation privilegierter Konten (Admins).
- **GPO-Mapping:** Erfassung aller GPOs, deren Status und Verknüpfungspunkte (Links) auf OUs/Sites.
- **Health-Check:** FSMO-Rolleninhaber, AD-Sites und Subnetz-Zuordnungen.

2.3 Netzwerk-Dienste & SNMP

_Für Netzwerkkomponenten (Switches, Drucker, USVs) und Non-Windows-Systeme._

- **SNMP v2c/v3:** Auslesen von System-Uptime, Hostname und Schnittstellen-Status (IF-MIB).
- **Netzwerk-Dienste:** DNS-Record-Validierung und DHCP-Scope-Auslastung (freie IPs vs. Reservierungen).

---

3. Workflow & Datenverarbeitung

4. **Discovery-Ablauf:**
    - Worker prüft Erreichbarkeit (Ping/Portcheck).
    - Priorisierung: CIM (Windows) -> SNMP (Network) -> Fallback (Generic Scan).
5. **Staging-Prozess:**
    - Die API empfängt den JSON-Payload.
    - **Delta-Abgleich:** Nur geänderte Attribute (z.B. neue Software, geänderte OU) werden versioniert im Audit-Log gespeichert.
6. **Auto-Klassifizierung:**
    - Assets werden basierend auf ihrem AD-Pfad (OU) oder SNMP-Device-Type automatisch kategorisiert (z.B. "Server", "VDI", "Switch").

---

4. Sicherheitsanforderungen

- **Credentials:** Nutzung von Least-Privilege-Accounts (Read-Only) aus dem verschlüsselten Vault.
- **Verschlüsselung:** Kommunikation zwischen Worker und API zwingend via TLS 1.3.
- **Datenschutz:** Möglichkeit, sensible AD-Attribute (z.B. Privatnummern) per Regex-Filter vom Import auszuschließen.

---

5. Abnahmekriterien (Definition of Done)

- **Performance:** Ein Standard-Server-Scan (CIM) dauert < 120 Sekunden.
- **AD-Navigation:** Die Web-App stellt die AD-OU-Struktur als navigierbaren Baum dar.
- **Berechtigungs-Check:** Suche nach einem User zeigt alle effektiven Gruppenmitgliedschaften (inkl. Verschachtelung).
- **Audit-Log:** Software-Deinstallationen oder OU-Verschiebungen werden als historische Events am Asset geloggt.
- **Protokoll-Resilienz:** Sauberes Error-Handling bei Protokoll-Timeouts (kein Scan-Abbruch bei einzelnen Fehlern).

---

6. API-Entwurf (Beispiel Payload)

json

```
{
  "scan_metadata": { "timestamp": "2023-10-27T10:00:00Z", "worker_id": "W-01" },
  "asset": {
    "hostname": "SRV-DB-01",
    "ad_path": "OU=SQL,OU=Servers,DC=company,DC=local",
    "cim_data": { "os": "Windows Server 2022", "ram_gb": 32 },
    "security": { "bitlocker": "Enabled", "privileged_group_member": true },
    "relations": { "manager": "CN=Admin-Joe,OU=Users...", "gpos": ["{GUID-1}", "{GUID-2}"] }
  }
}
```
