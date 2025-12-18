# ADR-0002: Azure Functions runtime startup stability settings

## Status
Accepted

## Date
2025-12-18

## Context
During initial deployment of the Azure Functions app (`func-atlasops-dev`) to Azure (Consumption plan),
the application intermittently returned HTTP 500 and Azure readiness checks reported:

- `azure.functions.script_host.lifecycle`: **Unhealthy**
- Description: **"Script host offline"**

The deployed package content in `site/wwwroot` appeared correct (host.json present, worker config and
functions metadata present), but the host became healthy only after subsequent retries/restarts.

This behaviour was observed shortly after:
- creating the Function App in a different region due to quota limits
- first GitHub Actions deployments
- initial platform provisioning/cold start

## Decision
We will explicitly configure runtime stability settings for Azure Functions to reduce ambiguity and
improve deterministic host startup.

## Settings to enforce
In Function App configuration (App Settings):

- `FUNCTIONS_WORKER_RUNTIME = dotnet-isolated`
- `FUNCTIONS_EXTENSION_VERSION = ~4`
- `WEBSITE_RUN_FROM_PACKAGE = 1`
- `AzureWebJobsFeatureFlags = EnableWorkerIndexing`

## Rationale
These settings:
- make the worker runtime explicit
- pin the Functions runtime major version
- reduce deployment/startup variability by running from the deployed package
- improve function discovery reliability for isolated worker indexing

## Consequences
- Slightly more configuration to manage across environments
- Improved startup determinism and easier troubleshooting
- Establishes a standard baseline for all environments (dev/test/prod)

## Follow-ups
- Add a dedicated readiness endpoint (`/api/ready`) to validate dependencies (Storage and Azure SQL)
- Capture any future startup incidents here with timestamps and the specific log/error signatures
