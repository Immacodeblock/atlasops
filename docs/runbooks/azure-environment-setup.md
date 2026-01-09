# Azure Environment Setup & Troubleshooting (AtlasOps)

This runbook documents how AtlasOps is deployed and operated in Azure,
including common issues encountered during initial setup and how they
were resolved.

---

## Azure subscription limits & provisioning behavior

During initial setup of AtlasOps, several issues were encountered that were
not related to application code or configuration, but to Azure platform
constraints and provisioning behavior.

---

### Dynamic VM quota (Azure Functions)

When creating the Azure Function App (Consumption plan), deployment failed
with errors similar to:

## Azure resources overview

AtlasOps uses the following Azure resources:

### Resource Group
- Name: `rg-atlasops-dev`
- Purpose: Development environment only

---

### Azure SQL Database
- Used for:
  - Workspaces
  - WorkItems
  - CheckRuns
  - ActivityEvents
- Access patterns:
  - Developer access via Azure Entra ID
  - Runtime access via Managed Identity

---

### Azure Function App
- Runtime: .NET 8 (isolated worker)
- Purpose:
  - HTTP APIs (create/read/update)
  - Queue-triggered background processing
- Key characteristics:
  - Stateless
  - Uses dependency injection
  - Uses Managed Identity for SQL access

---

### Azure Storage Account
- Purpose:
  - Azure Functions runtime storage
  - Azure Storage Queues for async processing
- Queues:
  - `workitem-checks` (primary)
  - `workitem-checks-poison` (dead-letter)

---

## Authentication & access model

### SQL authentication
- **Final state (recommended):**
  - Azure Functions authenticate using **Managed Identity**
  - Developers authenticate using **Azure Entra ID**
- **Temporary state (bootstrap only):**
  - SQL authentication was used briefly during early troubleshooting
  - Explicitly removed once Managed Identity was validated

---

### SQL firewall configuration (development)
- Developer public IP explicitly allowed
- “Allow Azure services and resources to access this server” enabled

Notes:
- This is acceptable for development
- Not suitable for production
- Production will require private endpoints or stricter firewall rules

---

## Application configuration in Azure

### Function App application settings
Key settings:
- `AzureWebJobsStorage`
- `FUNCTIONS_WORKER_RUNTIME`
- SQL connection string **not required** once Managed Identity is enabled

Important:
- Environment variables configured in the portal are authoritative
- Portal changes override local settings when deployed

---

## Local vs Azure behavior

| Concern | Local | Azure |
|------|------|------|
| Storage | Azurite | Azure Storage Account |
| SQL auth | Entra ID / SQL auth | Managed Identity |
| Queues | Local emulator | Real storage queues |
| Logs | Console | App Insights / log stream |

---

## Common issues & resolutions

### Function App deploys but returns HTTP 500
Cause:
- Function host not fully initialized
- Script host temporarily offline

Resolution:
- Wait for cold start to complete
- Check Function App logs
- Restart Function App if necessary

---

### Queue-trigger function not firing
Possible causes:
- Queue name mismatch
- Storage account misconfiguration
- Azurite running locally while Azure queue expected (or vice versa)

Resolution:
- Verify queue name exactly matches trigger attribute
- Confirm `AzureWebJobsStorage` value
- Check poison queue for failed messages

---

### Messages going straight to poison queue
Common causes:
- Message encoding mismatch (Base64)
- JSON deserialization failure
- Unhandled exceptions in worker

Resolution:
- Ensure queue client uses Base64 encoding
- Validate payload shape
- Add defensive logging + rethrow

---

### SQL connection errors after deployment
Common causes:
- Missing firewall rule
- Managed Identity not granted DB permissions
- App settings overwritten during redeploy

Resolution:
- Verify SQL firewall rules
- Confirm Managed Identity user exists in SQL
- Check Function App identity settings

---

## Observability tips

- Use Azure Storage Explorer to inspect queues
- Use Function App log stream for real-time debugging
- Avoid permanent verbose logging in production
- Enable detailed logging only during active troubleshooting

---

## Key lessons learned

- Azure Functions require storage even if app logic does not
- Local and cloud storage must be reasoned about separately
- Managed Identity simplifies runtime security significantly
- Queue-trigger pipelines require careful observability
- Many Azure issues are transient during cold start

This runbook exists to prevent rediscovery of these lessons.
