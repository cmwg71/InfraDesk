**Phase:** Stufe 3 (Mehrwertfunktionen)  
**Aufgabe:** Mobile Scan & Quick-Actions  
**ID:** 033  
**Status:** Enterprise-Ready

Aufgabenstellung: Mobile Infrastructure Management (MIM)

1. Beschreibung

Bereitstellung einer spezialisierten Web-App (PWA - Progressive Web App), die Technikern vor Ort den Zugriff auf CMDB-Daten via QR-Code ermöglicht. Der Fokus liegt auf maximaler Geschwindigkeit bei Routineaufgaben wie Inventur, Rollout und Standortwechsel.

2. Funktionsumfang (Enterprise-Standard)

2.1 Intelligenter Mobile-Scanner

- **Multi-Scan Mode:** Möglichkeit, mehrere QR-Codes hintereinander zu scannen (z. B. 20 Laptops auf einem Rollwagen), um eine Sammel-Aktion auszuführen.
- **Flashlight-Steuerung:** Integration der Kamera-LED für Scans in dunklen Server-Racks oder Lagerräumen.
- **Deep-Link Validation:** Die App prüft beim Scan den Security-Hash (aus T1_19), um unbefugtes Auslesen von URLs zu verhindern.

2.2 Kontextsensitive Quick-Actions

Basierend auf dem aktuellen Status des Assets bietet die App nur logisch sinnvolle Aktionen an:

- **Smart Rollout:** Scan eines Laptops im Lager -> Aktion: "An User ausgeben" (öffnet Suche für AD-Benutzer).
- **Inventory Mode:** Ein einfacher "Bestätigen"-Button setzt das `LastSeen`-Datum und den Bearbeiter-Stempel im Asset.
- **Foto-Upload:** Direkte Integration der Kamera zur Dokumentation von Transportschäden oder Rack-Einbauten (Direkt-Upload in Modul T1_10).

2.3 Standort- & Umzugslogik

- **Room-Mapping:** Scan eines Raum-Labels gefolgt von mehreren Asset-Scans ordnet alle Geräte sofort dem neuen Raum zu.
- **GPS-Tagging:** Optionales Speichern der Geokoordinaten beim Scan zur Verifizierung bei weitläufigen Geländen oder Außenstellen.

3. Technische Umsetzung & Sicherheit

3.1 PWA-Architektur (Offline-First)

- **Offline-Buffer:** Änderungen werden lokal gespeichert, falls im Serverraum kein WLAN/LTE verfügbar ist, und synchronisiert, sobald wieder Empfang besteht.
- **Responsive UI:** Optimierung für Einhand-Bedienung (große Buttons, Daumen-zentrisches Design).

3.2 Mobile Security

- **Conditional Access:** Zugriff nur über Firmengeräte oder via MFA (Multi-Faktor-Authentifizierung).
- **Device-Binding:** Verknüpfung der mobilen Session mit der Geräte-ID des Technikers.
- **Session-Limit:** Kürzere Timeouts für mobile Sessions im Vergleich zum Desktop-Client.

4. Workflow (Beispiel: Gerätetausch)

5. **Scan Alt-Gerät:** Techniker scannt defekten PC -> Status "Defekt/Abbau".
6. **Scan Neu-Gerät:** Techniker scannt Austauschgerät -> Status "Aktiv".
7. **Owner-Transfer:** App fragt: "Besitzer [User X] von Alt-Gerät übernehmen?" -> Bestätigung.
8. **Abschluss:** CMDB aktualisiert beide Assets; Benachrichtigungs-Engine (T1_09) informiert User X über den Tausch.

9. Abnahmekriterien

- **Geschwindigkeit:** Zeit vom Scan bis zur Anzeige der Quick-Actions beträgt < 1,5 Sekunden.
- **Feedback-Schleife:** Änderungen via Mobile werden sofort im Desktop-Client (WinUI) via SignalR visualisiert (z. B. Asset springt in der Liste von "Lager" auf "Aktiv").
- **Audit-Integrität:** Jede mobile Änderung ist im Audit-Log eindeutig als "Source: Mobile-App" und mit der Geräte-ID des Smartphones gekennzeichnet.
- **Fehlertoleranz:** Bei Fehlscans (z. B. QR-Code eines Drittanbieters) erfolgt eine klare, benutzerfreundliche Fehlermeldung statt eines Systemabsturzes.

---