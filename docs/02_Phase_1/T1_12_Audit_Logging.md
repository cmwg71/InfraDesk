**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Audit & Compliance (WORM - Write Once Read Many)  
**ID:** 012  
**Status:** Enterprise-Ready

Aufgabenstellung: Revisionssicheres Audit-Log (Enterprise Standard)

1. Beschreibung

Zentrales Logging-System zur lückenlosen Nachvollziehbarkeit aller Transaktionen. Das System garantiert, dass jede Änderung an Objekten (Assets, User, Tickets, IPAM) permanent dokumentiert wird. Dies dient sowohl der Fehleranalyse als auch dem Nachweis der Compliance.

2. Technisches Konzept (WORM-Prinzip)

2.1 Datenerfassung via Interceptor

- **Deep Tracking:** Erfassung auf Feldebene. Nicht nur "Objekt wurde geändert", sondern "Feld _RAM_ wurde von _16GB_ auf _32GB_ geändert".
- **Metadaten:**
    - **Zeitstempel:** Hochpräziser UTC-Zeitstempel.
    - **Akteur:** User-ID, Name und Quell-IP-Adresse.
    - **Kontext:** Action-Typ (`CREATE`, `UPDATE`, `DELETE`), Modul-ID und Trace-ID (um Änderungen über mehrere Tabellen hinweg einer Benutzeraktion zuzuordnen).

2.2 Revisionssicherheit & Integrität

- **Immutability (Unveränderbarkeit):** Die Datenbank-Rolle für den Standard-Betrieb hat keine `UPDATE`- oder `DELETE`-Berechtigungen auf die Audit-Tabellen.
- **Hashing (Optional für High-Security):** Jeder Log-Eintrag enthält einen Hash des vorherigen Eintrags (Chaining), um nachträgliche Manipulationen an der Datenbank-Datei erkennbar zu machen.
- **Soft-Delete Schutz:** Selbst wenn ein Objekt (z.B. ein Server) gelöscht wird, bleiben seine Audit-Logs dauerhaft erhalten.

3. Frontend & Sichtbarkeit (User Access)

3.1 Objekt-Historie (Timeline-View)

- **Direktzugriff:** Jedes Objekt (Asset-Detailseite, Ticket, IP-Adresse) erhält einen Reiter "Historie".
- **Transparenz:** Jeder berechtigte Nutzer kann sofort sehen: _Wer_ hat _wann_ _was_ geändert.
- **Visualisierung:** Farbliche Hervorhebung (Rot für alte Werte, Grün für neue Werte) in einer Diff-Ansicht.

3.2 Globale Audit-Suche (Admin)

- **Filter-Matrix:** Suche nach Zeitraum, Benutzer, spezifischer IP-Adresse oder Entitäts-Typ.
- **Export:** CSV- oder PDF-Export für Auditoren mit digitaler Signatur.

4. Workflow bei Änderungen

5. **Aktion:** Ein User ändert den Status eines Tickets auf "Closed".
6. **Interceptor:** Erkennt die Änderung im `DbContext`.
7. **Audit-Generierung:** Erstellt ein JSON-Delta der betroffenen Felder.
8. **Persistierung:** Schreibt den Log-Eintrag asynchron (um die Performance der UI nicht zu blockieren).

9. Abnahmekriterien

- **Vollständigkeit:** Alle Felder (außer Passwörter/Secrets) werden beim Update erfasst.
- **Löschresistenz:** Ein Versuch, einen Audit-Eintrag über die API oder das Frontend zu löschen, schlägt mit `403 Forbidden` fehl.
- **Performance:** Das Laden der Historie eines Assets mit 100+ Einträgen erfolgt in < 500ms.
- **Lesbarkeit:** Die JSON-Deltas werden für den Endanwender in natürliche Sprache übersetzt (z.B. "Status geändert von Offen zu Geschlossen").