Dateiname: T1_06_IAM_RBAC_Concept.md

Phase: Stufe 1 (Fundament)
Aufgabe: Identity & Access Management (IAM)
Beschreibung: Spezifikation der Multi-Mandantenfähigkeit und rollenbasierten Zugriffskontrolle.
ID: 006-Concept

# T1_06_IAM_RBAC_Concept.md

**Phase:** Stufe 1 (Fundament)  
**Aufgabe:** Identity & Access Management (IAM)  
**Beschreibung:** Spezifikation der Multi-Mandantenfähigkeit und rollenbasierten Zugriffskontrolle.  
**ID:** 006-Concept

# Konzeptionelle Vertiefung: IAM & RBAC

Dieses Dokument definiert die verbindlichen Regeln für den Datenzugriff und die Mandantentrennung innerhalb von InfraDesk.

## 1. Die Hierarchie-Ebenen

Das Sicherheitsmodell basiert auf drei Dimensionen:

*   **Mandant (Tenant):** Die oberste Trennung (z. B. Firma A, Tochtergesellschaft B). Ein Benutzer sieht standardmäßig nur CIs seines Mandanten.
*   **Rolle (Role):** Definiert, was getan werden darf (z. B. Reader, Editor).
*   **Scope (Geltungsbereich):** Definiert, auf welche CIs innerhalb eines Mandanten die Rolle wirkt.

## 2. Definition der Standard-Rollen

| Rolle | Berechtigungen (CRUD) | Beschreibung |
| :--- | :--- | :--- |
| **Global Admin** | C R U D + System | Voller Zugriff auf alle Mandanten, Systemkonfiguration und User-Management. |
| **Tenant Admin** | C R U D | Kann alle CIs innerhalb seines Mandanten verwalten und lokale User zuweisen. |
| **Configuration Manager** | C R U D | Darf CIs erstellen, bearbeiten und Beziehungen (Relations) pflegen. |
| **CI Editor** | R U | Darf bestehende CIs aktualisieren (z.B. Status), aber keine neuen Typen anlegen. |
| **Auditor / Reader** | R | Nur Lesezugriff auf CIs und Historie für Audits oder Support-Sichten. |

## 3. Mandanten-Trennungs-Logik (Data Segregation)

1.  **Impliziter Filter (EF Core):** Jede Datenbankabfrage wird vom `ApplicationDbContext` automatisch gefiltert. Ein User mit `Tenant_ID = 101` erhält bei `SELECT * FROM Assets` technisch immer ein unsichtbares `WHERE TenantId = 101`.
2.  **Shared Assets:** Es gibt einen dedizierten "Global Tenant" (ID: `00000000-0000-0000-0000-000000000000`). Assets in diesem Tenant (z. B. zentrale Core-Router) sind für alle Mandanten sichtbar (Read-Only), können aber nur von Global Admins bearbeitet werden.
3.  **Cross-Tenant Access:** Dienstleister (MSPs) können über eine Mapping-Tabelle (`UserTenants`) Berechtigungen für mehrere Mandanten erhalten. Der Tenant-Switcher im UI wechselt den aktiven Kontext (HTTP-Header `X-Tenant-ID`).

## 4. RBAC-Matrix für Multi-Mandanten

| Objekt | Global Admin | Tenant Admin | CI Editor (Tenant A) | Auditor (Tenant A) | CI Editor (Tenant B) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **System Settings** | Ja | Nein | Nein | Nein | Nein |
| **CI Daten (Tenant A)** | Ja | Ja | Ja (Update) | Ja (Read) | Kein Zugriff |
| **CI Daten (Tenant B)** | Ja | Nein | Kein Zugriff | Kein Zugriff | Ja (Update) |
| **User Mgmt (A)** | Ja | Ja | Nein | Nein | Nein |

## 5. Technische Implementierungsschritte

*   **Identity Provider (IdP):** Das System nutzt JWT (JSON Web Tokens). Externe Systeme wie Entra ID (Azure AD) können angebunden werden, wobei AD-Gruppen automatisch auf InfraDesk-Rollen gemappt werden.
*   **Audit-Trail:** Jede Aktion wird mit Zeitstempel, UserId und der im Kontext aktiven TenantId geloggt.
*   **Relations (Spezialfall):** Ein Asset von Mandant A darf nur dann eine Beziehung zu Mandant B haben, wenn das Ziel-Asset im "Global Tenant" liegt. Direkte Cross-Tenant-Relations zwischen isolierten Mandanten sind verboten, es sei denn, sie werden durch einen Global Admin explizit (via Override) erzwungen (z. B. geteiltes WAN).
