## Dateiname: T4_03_Offline_Mode.md

Phase: Stufe 4 (Expansion & Spezialthemen)

Aufgabe: Offline-Modus & Synchronisation

Beschreibung: Arbeiten ohne Netzwerkverbindung mit lokaler SQLite-Datenbank.

ID: 043

# Aufgabenstellung: Offline-Modus (Resilienz)

### Beschreibung

Ermöglicht den Betrieb der WinUI-App und der Mobile-App in Umgebungen ohne Netzwerk (z. B. abgeschirmte Serverräume oder Außeneinsätze).

### Funktionsumfang

1. **Lokaler Cache**:
    
    - Nutzung einer lokalen **SQLite**-Datenbank zur Speicherung ausgewählter Datensätze (Tickets, Assets).
        
2. **Delta-Synchronisation**:
    
    - Intelligenter Abgleich: Nur geänderte Daten werden übertragen, sobald eine Verbindung besteht.
        
3. **Konfliktmanagement**:
    
    - UI-Dialog zur Auflösung von Bearbeitungskonflikten (z. B. "Server A wurde offline und online gleichzeitig bearbeitet").
        

### Abnahmekriterien

- Die App bleibt bei Verbindungsverlust voll funktionsfähig für lesende und schreibende Zugriffe auf gecachte Daten.
    
- Die Synchronisation erfolgt automatisch im Hintergrund bei Wiederherstellung der Verbindung.