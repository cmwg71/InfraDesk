## Dateiname: T2_04_License_Management.md

Phase: Stufe 2 (Erweiterte Kernfunktionen) Aufgabe: Lizenzmanagement & Compliance-Indikatoren Beschreibung: Abgleich von installierter Software mit Soll-Zuweisungen. ID: 024

# Aufgabenstellung: SAM & Compliance-Anzeige

### Beschreibung

Verwaltung von Softwarelizenzen mit proaktiver Anzeige von Abweichungen zwischen Zuweisung und Scan-Ergebnis.

### Funktionsumfang

1. **Lizenz-Objekt-Verknüpfung**: Manuelle Zuordnung von Lizenzen zu Assets oder Usern.
    
2. **Compliance-Abgleich**:
    
    - Vergleich der manuell festgelegten Anzahl mit der durch den Discovery-Worker festgestellten Installationsanzahl.
        
3. **Mismatch-Indikator**:
    
    - Visuelle Hervorhebung in der UI, wenn `Manuelle Zuweisung != Automatische Erkennung`.
        
    - Detailansicht zeigt die Differenz pro Asset-Klasse an.
        

### Abnahmekriterien

- Das Dashboard zeigt eine Warnung bei Lizenz-Abweichungen.
    
- In der Asset-Ansicht ist sofort ersichtlich, ob die installierte Software durch zugewiesene Lizenzen gedeckt ist.