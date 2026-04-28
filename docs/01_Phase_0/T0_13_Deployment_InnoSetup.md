## Dateiname: T0_13_Deployment_InnoSetup.md Phase: Stufe 0 (Vorbereitung) Aufgabe: Deployment-Pipeline mit Inno Setup Beschreibung: Erstellung der Installationsdatei für den Client. ID: 013

# Aufgabenstellung: Client-Installer (Inno Setup)

### Beschreibung

Erstellung eines professionellen Windows-Installers für die InfraDesk WinUI-Anwendung.

### Details

1. **Build-Prozess**:
    
    - `dotnet publish` der WinUI-App als "Self-Contained" oder "Framework-Dependent".
        
    - Ausgabe als "Unpackaged EXE".
        
2. **Inno Setup Skript (.iss)**:
    
    - Definition des Installationspfads (Standard: `Program Files`).
        
    - Erstellung von Startmenü-Verknüpfungen und Desktop-Icons.
        
    - Einbindung von Abhängigkeiten (z.B. WebView2 Runtime, falls nötig).
        
3. **Update-Strategie**:
    
    - Der Installer muss erkennen, wenn eine Version bereits installiert ist, und diese überschreiben.
        

### Abnahmekriterien

- Eine `InfraDesk_Setup.exe` wird generiert.
    
- Die Anwendung lässt sich sauber installieren und über das Startmenü starten.
    
- Deinstallation über die Windows-Systemsteuerung funktioniert einwandfrei.