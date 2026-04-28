## Dateiname: T4_08_Kubernetes_Scaling.md

Phase: Stufe 4 (Expansion & Spezialthemen)

Aufgabe: Container-Orchestrierung & Lastverteilung

Beschreibung: Deployment-Strategie für hochverfügbare, skalierbare Cloud-Umgebungen.

ID: 048

# Aufgabenstellung: Kubernetes & Cloud-Skalierung

### Beschreibung

Für sehr große Organisationen mit Zehntausenden Assets muss die InfraDesk-Infrastruktur horizontal skalierbar sein.

### Funktionsumfang

1. **Containerisierung**:
    
    - Erstellung optimierter Dockerfiles für API, Worker und Web-Portal.
        
    - Multi-Stage Builds zur Reduzierung der Image-Größe.
        
2. **Kubernetes (K8s) Manifeste**:
    
    - Deployment-Definitionen mit `HorizontalPodAutoscaler` (HPA) basierend auf CPU/RAM-Last.
        
    - Service-Definitionen für internes Load-Balancing.
        
3. **State-Management**:
    
    - Nutzung von **Redis** als verteilter Cache für SignalR-Backplane und Session-Daten.
        
    - Konfiguration von `Ingress`-Controllern für TLS-Terminierung und Pfad-Routing.
        
4. **Health-Checks**:
    
    - Implementierung von `/health/ready` und `/health/live` Endpunkten im Backend.
        

### Abnahmekriterien

- Die API lässt sich auf mehrere Instanzen skalieren, wobei SignalR-Benachrichtigungen (Modul 09) konsistent an alle Clients verteilt werden.
    
- Ein Rolling-Update in Kubernetes verursacht keine Downtime für die WinUI-Clients.