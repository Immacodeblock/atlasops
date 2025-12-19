# ADR-0005: Use Managed Identity for Azure SQL authentication

## Status
Accepted

## Date
2025-12-18

## Context
AtlasOps initially connected to Azure SQL Database using SQL authentication with a
dedicated application user during early development.

While functional, this approach required storing credentials in application
configuration and managing secrets explicitly.

Azure provides Managed Identity as a first-class mechanism for Azure-hosted
workloads to authenticate to other Azure services without credentials.

## Decision
We will use a **system-assigned Managed Identity** on the Azure Functions app
to authenticate to Azure SQL Database using Microsoft Entra ID.

A database user mapped to the Function App’s managed identity is created using:

- `CREATE USER FROM EXTERNAL PROVIDER`

The managed identity is granted least-privilege access to the database.

## Rationale
- Eliminates the need to store or rotate database credentials
- Reduces security risk and operational overhead
- Aligns with Azure-native, enterprise security best practices
- Simplifies configuration across environments
- Makes secret leakage via logs, config, or CI/CD impossible

## Consequences
Positive:
- Passwordless, secure database authentication
- Cleaner application configuration
- Easier future compliance and audits
- Clear separation between human and application identities

Trade-offs:
- Requires Entra ID configuration on the SQL Server
- Slightly more complex initial setup compared to SQL auth

## Follow-ups
- Remove legacy SQL-auth users once confidence is established
- Use the same pattern for any additional Azure services
- Consider Key Vault only for non-Azure or external secrets
