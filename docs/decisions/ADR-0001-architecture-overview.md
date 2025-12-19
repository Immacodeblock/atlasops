# ADR-0001: AtlasOps high-level architecture and technology choices

## Status
Accepted

## Date
2025-12-18

## Context
AtlasOps is intended to be a senior-level, cloud-native portfolio project that demonstrates:
- real-world Azure engineering
- cost-aware architectural choices
- secure-by-default patterns
- operational maturity (CI/CD, readiness, diagnostics)

The system needed to be:
- feasible within a Free Trial / low-cost environment
- realistic and representative of production systems
- suitable for iterative expansion (data model, identity, observability, frontend)

## Decision
We will build AtlasOps using the following high-level architecture:

- **Compute:** Azure Functions (Consumption plan, .NET 8 isolated)
- **Data:** Azure SQL Database
- **Identity:** Microsoft Entra ID (with an initial SQL-auth bootstrap, migrating to Managed Identity)
- **CI/CD:** GitHub Actions (source-controlled deployments)
- **Configuration:** Azure App Settings (migrating to Key Vault references)
- **Health model:** Explicit liveness (`/health`) and readiness (`/ready`) endpoints

## Rationale
- Azure Functions provide serverless compute with low operational overhead and cost efficiency.
- .NET isolated worker aligns with modern Azure Functions direction and long-term support.
- Azure SQL leverages existing SQL expertise and provides a fully managed relational store.
- GitHub Actions provides transparent, auditable CI/CD without additional tooling.
- Explicit health/readiness endpoints support reliable operations and diagnostics.
- The architecture mirrors patterns used in enterprise Azure environments while remaining accessible for a portfolio project.

## Consequences
Positive:
- Minimal baseline cost
- Clear separation of concerns
- Strong alignment with Azure-native best practices
- Easy evolution toward more advanced patterns (Managed Identity, Private Endpoints, Key Vault)

Trade-offs:
- Serverless cold starts must be managed explicitly
- Cross-region deployment (Functions vs SQL) during bootstrap
- Some production-grade security patterns deferred to later iterations

## Follow-ups
- Introduce Managed Identity for SQL authentication
- Add observability and structured logging
- Define a data model and migration strategy
- Introduce a frontend (Static Web App) once backend stabilises
