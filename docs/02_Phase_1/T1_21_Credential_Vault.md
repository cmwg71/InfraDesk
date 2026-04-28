## Dateiname: T1_21_Credential_Vault.md

Phase: Stufe 1 (Fundament) Aufgabe: Credential-Tresor & Passwort-Manager Deep-Dive Beschreibung: Detaillierte Integration externer Passwort-Manager und Secret-Management. ID: 021

# Aufgabenstellung: Credential-Tresor & PWM-Integration (Deep-Dive)

### Beschreibung

Der Credential-Tresor von InfraDesk muss flexibel genug sein, um entweder als eigenständiger Speicher zu dienen oder als nahtloses Gateway zu bestehenden Enterprise-Passwort-Managern (PWM). Ziel ist es, dass der Discovery-Worker Credentials "Just-in-Time" abruft, ohne dass diese dauerhaft in der InfraDesk-Datenbank verbleiben.

### 1. Technische Integrationspfade

#### A. Bitwarden / Vaultwarden (API & CLI)

- **Anbindung**: Nutzung der Bitwarden Public API für Cloud-Instanzen oder der Bitwarden CLI für On-Prémise Vaultwarden-Instanzen.
    
- **Authentifizierung**: Verwendung von `Client Secret` und `Client ID`.
    
- **Abruf**: Gezieltes Suchen nach Einträgen via UUID oder Tags (z.B. Tag: `InfraDesk-Scan`).
    

#### B. KeePass / KeePass XC (.kdbx)

- **Anbindung**: Da KeePass lokal basiert ist, erfolgt der Zugriff über `KPScript.exe` oder die `KeepassHttp` API.
    
- **Szenario**: Der Discovery-Worker erhält Zugriff auf eine verschlüsselte `.kdbx` Datei im Netzwerk.
    
- **Sicherheit**: Das Master-Passwort für die Datei wird im verschlüsselten Windows-Zertifikatsspeicher des Worker-Nodes abgelegt.
    

#### C. 1Password (Connect API)

- **Anbindung**: Nutzung des `1Password Connect SDK`.
    
- **Voraussetzung**: Ein im Kundennetzwerk laufender `1Password Connect Server`.
    
- **Vorteil**: Bietet eine native REST-API für den Zugriff auf Vaults innerhalb der Infrastruktur.
    

### 2. Dynamisches Field-Mapping

Da PWM-Einträge unterschiedlich aufgebaut sein können, benötigt InfraDesk ein Mapping-System:

- **Standard-Felder**: Username -> `SSH_User`, Password -> `SSH_Password`.
    
- **Custom Fields**: Mapping von PWM-Zusatzfeldern auf InfraDesk-Bedürfnisse (z.B. Feld `Sudo-Pass` in Bitwarden -> `Sudo_Credential` in InfraDesk).
    
- **SSH-Keys**: Unterstützung für den Abruf von Private-Keys aus den "File Attachments" oder "Notes"-Sektionen der Passwort-Manager.
    

### 3. Sicherheits-Architektur (Zero-Knowledge Ansatz)

- **Keine Persistenz**: Passwörter aus externen PWMs werden niemals in der PostgreSQL-Datenbank von InfraDesk gespeichert.
    
- **In-Memory Only**: Secrets existieren nur flüchtig im Arbeitsspeicher des Discovery-Workers während der aktiven Session.
    
- **Scoping**: Credentials können auf bestimmte IP-Bereiche, Asset-Kategorien oder Standorte eingeschränkt werden (z.B. "KeePass-DB-A nur für Server im Standort 'Berlin'").
    

### 4. Workflow des Abrufs

1. **Discovery-Trigger**: Worker startet Scan für IP `192.168.1.50`.
    
2. **Lookup**: API prüft: "Welche Credentials sind für diese IP hinterlegt?" -> Ergebnis: "Bitwarden Eintrag UUID: 123-abc".
    
3. **Fetch**: Worker fragt über den Credential-Service den PWM an -> Erhält Secret.
    
4. **Action**: Worker nutzt Secret für WMI/SSH-Login.
    
5. **Purge**: Secret wird nach Scan-Abschluss aus dem Memory gelöscht.
    

### Abnahmekriterien

- **Konnektivität**: Erfolgreicher Test-Verbindungsaufbau zu allen drei Systemen (Bitwarden, KeePass, 1Password).
    
- **Mapping-Logik**: Korrekte Zuweisung von Custom-Fields zu Scan-Parametern.
    
- **Revisionssicherheit**: Jeder Abruf eines Secrets wird im Audit-Log von InfraDesk (T1_12) dokumentiert, ohne das Secret selbst zu loggen.