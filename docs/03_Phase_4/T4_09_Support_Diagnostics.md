## Dateiname: T4_09_Support_Diagnostics.md

Phase: Stufe 4 (Expansion & Spezialthemen) Aufgabe: Support-, Diagnose- & Lizenz-Werkzeuge Beschreibung: Integrierte Tools zur Fehleranalyse, Systemüberwachung und Lizenzierung. ID: 049

# Aufgabenstellung: Enterprise Support, Diagnostics & Licensing

### Beschreibung

In großen Umgebungen ist die Fehlersuche komplex, und der Schutz des geistigen Eigentums der Software essenziell. Dieses Modul bietet Administratoren Werkzeuge zur Selbstüberwachung von InfraDesk sowie eine manipulationssichere Lizenzierungslogik.

### Funktionsumfang

1. **Zentrales Log-Viewer**:
    
    - Ein spezialisierter Bereich in WinUI zum Durchsuchen der technischen System-Logs (Serilog).
        
    - Filter nach Korrelations-IDs, um eine Anfrage vom Client durch die API bis zum Worker zu verfolgen.
        
2. **Worker-Monitoring**:
    
    - Live-Ansicht: Welcher Discovery-Worker scannt gerade welches Subnetz?
        
    - Anzeige von Performance-Metriken (Scan-Dauer, Erfolgsquote).
        
3. **Integrierter Support-Assistent**:
    
    - Export von verschlüsselten Diagnose-Paketen (Logs + anonymisierte Config) für den Hersteller-Support.
        
4. **Datenbank-Statistik**:
    
    - Anzeige der Tabellengrößen und Index-Nutzung (PostgreSQL-Health).
        
5. **App-Lizenzierung & Trial-Logik (Hybrid-Modell)**:
    
    - **Trial-Periode**: 30-Tage-Testphase. Der Startzeitpunkt wird beim ersten Start verschlüsselt in der Datenbank und zusätzlich in einem versteckten "Anchor-File" im Filesystem oder der Registry hinterlegt, um Löschversuche der DB zu erkennen.
        
    - **Online-Aktivierung (Standard)**: Die App sendet die Hardware-ID (HWID) an einen zentralen Lizenzserver. Dieser validiert die ID und sendet ein zeitlich begrenztes, signiertes Token zurück.
        
    - **Offline-Aktivierung (Fallback)**: Für Air-Gapped-Systeme generiert InfraDesk eine `activation_request.req` Datei. Der Admin lädt diese auf einem internetfähigen PC hoch und erhält eine signierte `license.lic` Datei zurück, die manuell importiert wird.
        
    - **Kryptographische Absicherung**: Verwendung von RSA-2048 Signaturen. Die Anwendung prüft bei jedem Start mit dem integrierten Public-Key, ob die Lizenzdatei manipuliert wurde.
        
    - **Hardware-Bindung**: Erzeugung eines Fingerprints aus CPU-ID, Mainboard-Seriennummer und MAC-Adresse.
        
    - **Anti-Tamper & Zeit-Schutz**:
        
        - **Online**: Abgleich der Systemzeit mit einem NTP-Server oder dem Lizenzserver.
            
        - **Offline**: Das System speichert den "Last-Run"-Zeitstempel. Wenn die aktuelle Systemzeit hinter diesem Zeitstempel liegt, wird die Lizenz wegen Manipulationsverdacht gesperrt.
            

### Abnahmekriterien

- Ein Administrator kann innerhalb der App sehen, warum ein bestimmter Discovery-Job fehlgeschlagen ist.
    
- Diagnose-Pakete enthalten keine Passwörter oder sensiblen Asset-Daten (DLP-Check).
    
- Die Anwendung erkennt den Ablauf der 30-tägigen Testphase (auch ohne Internet) und fordert zur Aktivierung auf.
    
- Ein Lizenz-Key lässt sich nur auf der Hardware aktivieren, für die der Request generiert wurde.