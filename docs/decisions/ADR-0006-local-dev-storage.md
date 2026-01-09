
---

## ADR to capture the “big” local-dev storage decisions


# ADR-0006: Local development uses Azurite for Azure Storage (Queues)

## Status
Accepted

## Context
AtlasOps uses Azure Storage Queues for async processing (`workitem-checks` queue) via an Azure Functions queue trigger.
When running locally, the Functions host still requires an `AzureWebJobsStorage` setting for triggers and internal runtime behaviors.

## Decision
For local development we will use **Azurite** (local Azure Storage emulator) and set:
- `AzureWebJobsStorage = UseDevelopmentStorage=true`

We will use **Azure Storage Explorer** for inspecting local queues when debugging.

## Consequences
- Developers must run Azurite locally for queue-trigger development/testing.
- Local queue names and contents are independent from Azure queue names and Azure storage accounts.
- Production uses a real Azure Storage Account provided via Function App settings.

## Notes / Implementation details
- Queue message encoding must be compatible with the Functions queue trigger.
- The producer uses `QueueClientOptions.MessageEncoding = Base64` to avoid decoding failures.
