## Dateiname: T1_05_IPAM_Basics.md

**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** IP-Adressverwaltung & DHCP-Monitoring (Netbox-Style)  
**ID:** 005  
**Status:** Enterprise-Refined

Aufgabenstellung: Enterprise IPAM & DHCP-Logik

1. Beschreibung

Zentrale Verwaltung der IP-Infrastruktur nach dem Vorbild von Netbox. Das System dient als autoritative Quelle für alle IP-Ressourcen. Es kombiniert die logische Netzplanung mit der realen Überwachung von DHCP-Reservierungen durch den Discovery-Worker.

2. Funktionsumfang (Enterprise-Standard)

2.1 Netzhierarchie & VRF (Netbox-Modell)

- **VRFs (Virtual Routing and Forwarding):** Unterstützung für überlappende IP-Bereiche durch Trennung in verschiedene Routing-Instanzen (essentiell für getrennte Netze wie Produktion vs. Labor).
- **Aggregates & Prefixes:** Definition von großen Adressblöcken (Aggregates, z.B. RFC1918) und deren Unterteilung in Prefixes (Subnetze).
- **Status-Tracking:** Jeder Prefix hat einen Status (`Active`, `Reserved`, `Deprecated`).
- **Utilization:** Echtzeit-Berechnung der Belegung basierend auf IP-Objekten innerhalb des Prefixes.

2.2 DHCP-Monitoring & Validierung

- **Scope-Synchronisation:** Manuelle Definition von Pools innerhalb von Prefixes.
- **Reservierungs-Audit:** Der Worker liest DHCP-Reservierungen aus und verknüpft sie mit den `IP Address`-Objekten in der CMDB.
- **Conflict Detection:** Markierung von Abweichungen, wenn eine DHCP-Reservierung existiert, die im IPAM nicht als "Belegt" markiert ist (Shadow IP-Usage).

2.3 Multi-Tenancy & Governance

- **Tenants (Mandanten):** Zuordnung von Prefixes und IP-Blöcken zu Abteilungen, Kunden oder Standorten.
- **Rollenbasierte Berechtigung (RBAC):** Feingranulare Steuerung, wer IPs in welchen Netzen reservieren oder ändern darf.
- **Naming Conventions:** Validierung von Hostnames gegen definierte Unternehmensrichtlinien bei der IP-Vergabe.

2.4 IP-Status & Lifecycle

- **Single IP Management:** Jede IP ist ein Objekt mit Attributen (DNS-Name, Beschreibung, Device-Link, NAT-Inside/Outside).
- **Lease-Ausschluss:** Dynamische Leases werden als "Ephemeral" (flüchtig) klassifiziert und **nicht** in die permanente Datenbank übernommen.

3. Workflow (Netbox-Style)

4. **Prefix-Zuweisung:** Administrator reserviert den "Next Free Prefix" innerhalb eines Aggregates.
5. **IP-Zuweisung:** Für ein neues Asset wird die "First Free IP" innerhalb eines Prefixes vorgeschlagen.
6. **Worker-Abgleich:** Der Worker prüft, ob die am DHCP-Server hinterlegte MAC-Adresse mit der MAC des Assets in der CMDB korreliert.

7. Enterprise-Features (Zusatz zu Netbox)

- **API-First:** Vollständige REST-API zur Integration in Provisionierungs-Skripte (z.B. Ansible/Terraform).
- **Audit-Trail:** Lückenlose Historie (Wer hat wann welche IP verschoben/geändert?).
- **Reporting:** Automatisierte Berichte über IP-Knappheit in kritischen Netzen.

5. Abnahmekriterien

- **Netbox-Parität:** Die Struktur von Aggregates, Prefixes und IP-Adressen folgt der Netbox-Logik.
- **VRF-Isolierung:** IPs können in unterschiedlichen VRFs identisch sein, ohne einen Datenbank-Konflikt auszulösen.
- **Worker-Präzision:** DHCP-Reservierungen werden korrekt importiert und als solche visualisiert.
- **Performance:** Schnelle Suche über Millionen von IP-Datensätzen via Indexing.

---