**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Enterprise Global Search (PostgreSQL GIN/FTS)  
**ID:** 011  
**Status:** Enterprise-Ready

Aufgabenstellung: Globale Suche & Kontext-Indizierung

1. Beschreibung

Implementierung einer zentralen Such-Engine ("Omni-Search"), die alle Module (CMDB, Ticketing, IPAM, Dokumente) indiziert. Das System muss in der Lage sein, sowohl strukturierte Daten (Tabellenspalten) als auch unstrukturierte Daten (JSONB-Attribute und Ticket-Beschreibungen) in Millisekunden zu durchsuchen.

2. Technisches Design & Indexierung

2.1 Multi-Vektor-Suche (Weighting)

Wir nutzen PostgreSQL **Full-Text-Search (FTS)** mit gewichteten Vektoren (`tsvector`), um die Relevanz zu steuern:

- **Gewicht A (Hoch):** Hostnames, Asset-Tags, Ticket-IDs, IP-Adressen, Namen.
- **Gewicht B (Mittel):** Titel von Tickets, OU-Pfade, Software-Namen.
- **Gewicht C (Niedrig):** Ticket-Beschreibungen, Journal-Einträge, JSONB-Metadaten.

2.2 JSONB-Deep-Search

- **Dynamic Indexing:** Der Discovery-Worker-Output (JSONB) wird über einen `GIN`-Index (Generalized Inverted Index) durchsuchbar gemacht.
- **Key-Value-Awareness:** Die Suche erkennt Schlüsselbegriffe innerhalb der Scan-Ergebnisse (z.B. "Bitlocker" oder eine spezifische "CPU-Modellnummer").

2.3 Trigram-Suche (Fuzzy Search)

- Zusätzlich zur Wortsuche wird die Erweiterung `pg_trgm` genutzt, um **Teilstring-Suchen** und **Tippfehler-Toleranz** (Fuzzy Matching) zu ermöglichen (z.B. findet "Srv-Prod" auch "Server-Production").

3. Enterprise-Funktionen

3.1 Security-Filtering (RBAC)

- **Scope-Aware Search:** Suchergebnisse werden zur Laufzeit gefiltert. Ein User sieht in den Suchergebnissen nur die Tickets und Assets, für die er laut Berechtigungsmatrix (Modul 001/004) Zugriff hat.
- **Tenant-Isolierung:** In Multi-Tenancy-Umgebungen werden Ergebnisse strikt pro `TenantID` getrennt.

3.2 Quick-Actions (Cmd+K Logik)

Die API liefert neben dem Treffer auch "Actions" zurück:

- Suche nach einer IP -> Direktlink zu "Ping Tool" oder "RDP starten".
- Suche nach Ticket-ID -> Direktlink zu "Status ändern".

3.3 Search-Suggestions (Type-ahead)

- Während der Eingabe liefert die API bereits erste Top-Treffer aus dem Index, um die Navigationszeit zu minimieren.

4. Workflow

5. **Indizierung:** Bei jedem `Insert` oder `Update` (via DB-Trigger oder EF Core Interceptor) wird der Such-Vektor der Entität aktualisiert.
6. **Abfrage:** Der User gibt einen Suchbegriff in der WinUI-App ein.
7. **Ranking:** PostgreSQL berechnet das Ranking (`ts_rank`) basierend auf der Gewichtung.
8. **Grouping:** Die API gruppiert Ergebnisse nach Typ (z.B. "3 Assets, 1 Ticket, 5 IP-Adressen").

9. Abnahmekriterien

- **Performance:** Komplexe Abfragen über 100.000 Datensätze liefern Ergebnisse in < 250ms.
- **JSONB-Integrität:** Ein Wert, der tief in einem Scan-Ergebnis eines Workers versteckt ist (z.B. eine Seriennummer), wird zuverlässig gefunden.
- **Relevanz:** Bei Suche nach einem Hostnamen steht das Asset ganz oben, nicht ein Ticket, in dem der Hostname nur am Rande erwähnt wird.
- **Highlighting:** Die API liefert "Snippets" zurück, die zeigen, wo der Suchbegriff im Text gefunden wurde (Fettdruck-Markierung).

---