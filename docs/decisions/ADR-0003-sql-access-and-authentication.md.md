# ADR-0003: Azure SQL access and authentication strategy (development)

## Status
Accepted

## Date
2025-12-18

## Context
AtlasOps requires reliable access to Azure SQL Database from:
- developer workstations (schema creation, migrations, troubleshooting), and
- Azure Functions (runtime execution, readiness checks, async processing).

By default, Azure SQL Server blocks all inbound connections via firewall rules and
requires explicit authentication configuration.

During early development, initial connectivity attempts failed due to:
- missing firewall allow rules, and
- evolving authentication mechanisms (SQL auth → Entra ID → Managed Identity).

This ADR documents the **development-time access strategy**, acknowledging that
production will require a stricter posture.

---

## Decision
For the **development environment**, we adopt a staged SQL access strategy:

### 1. Network access (development only)
- Add the developer workstation’s public IPv4 address to the SQL Server firewall.
- Enable **“Allow Azure services and resources to access this server”**.

### 2. Authentication
- Use **Azure Entra ID authentication** for interactive developer access.
- Use **Managed Identity** for Azure Functions runtime access.
- Avoid embedding SQL usernames/passwords in application configuration.

### 3. Transitional allowance (time-bound)
- SQL authentication was temporarily used during bootstrap and troubleshooting.
- This was explicitly replaced once Managed Identity was confirmed working.

---

## Rationale
- Firewall rules are required to unblock development access in a cost-constrained,
  non-VNet environment.
- “Allow Azure services…” avoids brittle management of dynamic outbound IPs
  for Azure Functions during early development.
- Managed Identity provides:
  - passwordless authentication,
  - reduced secret handling,
  - closer alignment with production best practices.
- A staged approach allowed progress without prematurely introducing VNet or
  Private Endpoint complexity.

---

## Consequences

### Positive
- Reliable SQL access for developers and Azure Functions.
- Clear separation between **development convenience** and **production intent**.
- Elimination of connection strings with embedded credentials.
- Easier local debugging and cloud parity once identity was stabilized.

### Negative / Trade-offs
- “Allow Azure services…” is broader than a production-grade firewall policy.
- Developer IP allowlisting requires updates when public IPs change.
- Temporary SQL auth usage increased short-term configuration complexity.

These trade-offs are explicitly accepted for development and are time-bound.

---

## Alternatives Considered
- **Allowlisting Azure Functions outbound IPs**
  - More restrictive, but operationally fragile during early iteration.
- **Private Endpoint + VNet integration**
  - Strongest security posture, but excessive cost/complexity for development.
- **Disabling public access entirely**
  - Incompatible with early bootstrap and diagnostics.

---

## Follow-ups
- Remove “Allow Azure services…” when moving to production.
- Replace firewall-based access with:
  - Private Endpoint + VNet integration.
- Enforce Managed Identity as the only runtime authentication mechanism.
- Document production SQL posture in a separate ADR.
