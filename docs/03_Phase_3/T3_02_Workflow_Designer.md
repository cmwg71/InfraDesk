## Dateiname: T3_02_Workflow_Designer.md

Phase: Stufe 3 (Mehrwertfunktionen) Aufgabe: Workflow-Designer mit Eskalationslogik Beschreibung: Modellierung von Prozessen inklusive Vertretungs- und Eskalationsregeln. ID: 032

# Aufgabenstellung: Workflow-Designer (Eskalation & Vertretung)

### Beschreibung

Tool zur Modellierung von IT-Prozessen, das Ausfallsicherheit durch Rollen-Hierarchien garantiert.

### Funktionsumfang

1. **Rollen-Management**: Jede Prozess-Rolle (z.B. "IT-Leiter") erhält ein Feld für eine primäre Vertretung.
    
2. **Eskalations-Stufen**:
    
    - Definition von Timeouts pro Workflow-Schritt.
        
    - Bei Zeitüberschreitung: Automatische Weiterleitung an die nächsthöhere Ebene in der Organisationsstruktur.
        
3. **Status-Tracking**: Transparente Anzeige, in welcher Eskalationsstufe sich ein Vorgang befindet.
    

### Abnahmekriterien

- Ein Workflow leitet bei Abwesenheit/Timeout korrekt an die Vertretung oder die nächste Ebene weiter.
    
- Eskalationsereignisse werden separat im Audit-Log vermerkt.