## Dateiname: T0_01_Architecture_Setup.md Phase: Stufe 0 (Vorbereitung) Aufgabe: Projektstruktur & Clean Architecture Beschreibung: Initialisierung der .NET 8 Projektstruktur nach Clean Architecture Prinzipien. ID: 001

# Aufgabenstellung: Projekt-Setup

### Beschreibung

Einrichtung der Solution-Struktur zur Gewährleistung der Trennung von Belangen.

### Architektur-Komponenten

## Dateiname: T0_01_Architecture_Setup.md

Phase: Stufe 0 (Vorbereitung) Aufgabe: Projektstruktur & MVVM Setup Beschreibung: Initialisierung der .NET 8 Solution nach Clean Architecture und MVVM. ID: 001

# Aufgabenstellung: Projekt-Setup & MVVM-Struktur

### Beschreibung

Einrichtung der Solution-Struktur zur Gewährleistung der Trennung von Belangen unter Einhaltung des MVVM-Patterns im Frontend.

### Architektur-Komponenten

1. **InfraDesk.Core**: Domänen-Entitäten und Kern-Logik (Models).
    
2. **InfraDesk.Application**: Interfaces, DTOs und Use Cases.
    
3. **InfraDesk.UI.WinUI**:
    
    - **Views/**: Alle XAML-Pages (MainPage, AssetDetailPage etc.).
        
    - **ViewModels/**: Zugehörige ViewModels (Nutzung von `ObservableProperty` und `RelayCommand`).
        
    - **Services/**: NavigationService, DialogService zur Entkopplung der ViewModels von WinUI-spezifischen Aufrufen.
        
4. **InfraDesk.Infrastructure**: EF Core Anbindung und Passwort-Tresor-Implementierung.
    

### MVVM-Leitlinien

- **Data Binding**: Strikte Nutzung von `{x:Bind}` in den Views auf das ViewModel.
    
- **Commands**: Keine Event-Handler im Code-Behind; Nutzung von Commands im ViewModel.
    
- **DI Container**: Zentrale Registrierung aller Services und ViewModels im `IHost` der App.
    

### Abnahmekriterien

- Solution kompiliert fehlerfrei.
    
- Eine Test-View zeigt Daten aus einem Mock-ViewModel an, die über einen Service injiziert wurden.

### Abnahmekriterien

- Solution kompiliert.
    
- Abhängigkeiten fließen nur nach "innen" (UI -> API -> App -> Core).
    
- Swagger-UI ist unter `/swagger` erreichbar.