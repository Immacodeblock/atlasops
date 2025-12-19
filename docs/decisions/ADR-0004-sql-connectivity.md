# ADR-0004: Azure SQL connectivity from Azure Functions

## Status
Accepted

## Date
2025-12-18

## Context
The AtlasOps API requires persistent relational storage. Azure SQL Database was chosen to align
with existing SQL expertise and Azure-native managed services.

Connectivity from an Azure Functions (Consumption, .NET isolated) app needed to be validated
early to surface firewall, authentication, and configuration issues.

## Decision
- Use a dedicated application database user (`atlasops_app`) with least-privilege permissions
- Store SQL connection string in Azure Function App Application Settings
- Implement a readiness endpoint (`/api/ready`) that validates SQL connectivity via `SELECT 1`
- Return HTTP 503 when dependencies are unavailable

## Rationale
- Separates human/admin access from application access
- Avoids embedding secrets in source control
- Enables deterministic startup validation and safer deployments
- Mirrors enterprise patterns used in regulated environments

## Consequences
- Initial use of SQL authentication (password-based)
- Planned migration to Entra-based Managed Identity for passwordless access

## Follow-ups
- Migrate database authentication to Managed Identity
- Replace direct connection string with Azure Key Vault reference
- Expand readiness checks to include schema validation
