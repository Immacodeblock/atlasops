# ADR-0003: Azure SQL firewall configuration for development access

## Status
Accepted

## Date
2025-12-18

## Context
During early development of AtlasOps, connectivity to Azure SQL Database was required from:
- a developer workstation (for schema creation, validation, and troubleshooting), and
- an Azure Functions application (for runtime readiness and dependency validation).

Initial connection attempts failed due to Azure SQL Server firewall restrictions, which block
all inbound connections by default unless explicitly permitted.

At this stage of the project, the system is operating in a **development environment** under
Free Trial and low-cost constraints. The priority was to enable reliable developer and
Azure-hosted access with minimal operational overhead, while still acknowledging longer-term
security best practices.

This decision applies to the **development environment only** and is expected to be revisited
when moving toward a production-like security posture.

## Decision
For the development environment, we will configure Azure SQL Server networking as follows:

1. Add the developer workstation’s current public IPv4 address to the SQL Server firewall rules.
2. Enable the SQL Server setting: **“Allow Azure services and resources to access this server.”**

## Rationale
- Adding the developer IP enables immediate access for schema work, diagnostics, and validation.
- Enabling “Allow Azure services…” allows Azure Functions to connect without managing dynamic or
  changing outbound IP addresses during early development.
- This approach balances security awareness with delivery velocity in a constrained environment.
- It avoids prematurely introducing higher-complexity solutions (e.g. Private Endpoints)
  before the system architecture stabilises.

## Consequences
Positive:
- Reliable connectivity for both developer tools and Azure Functions.
- Reduced setup friction during early-stage development.
- Faster feedback loops when validating infrastructure and application changes.

Negative / Trade-offs:
- “Allow Azure services and resources…” provides broader access than a production-grade posture.
- Developer IP-based firewall rules may need updates when public IPs change.

These trade-offs are accepted for development and are explicitly time-bound.

## Alternatives Considered
- **Allowlisting Azure Functions outbound IPs:** More restrictive, but operationally brittle and
  harder to manage during early development.
- **Private Endpoint + VNet integration:** Strongest security posture, but significantly higher
  complexity and cost for a development environment.
- **Disabling public access entirely:** Not compatible with early bootstrap and troubleshooting.

## Follow-ups
- Revisit SQL Server network posture when moving toward a production-like environment.
- Replace broad firewall access with one of:
  - Private Endpoint + VNet integration, or
  - Tightly scoped firewall rules.
- Migrate Azure Functions authentication to **Managed Identity** for passwordless SQL access.
