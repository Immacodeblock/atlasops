# Logging & Verbosity Runbook (AtlasOps)

This runbook documents how logging verbosity is configured for AtlasOps,
why temporary changes may be made during debugging, and how to safely
enable/disable verbose logs.

---

## Background

AtlasOps runs on **Azure Functions (isolated worker)**.

Logging is controlled by:
- `host.json` → runtime + trigger logging
- `local.settings.json` → environment values (local only)
- Function host startup flags (e.g. `--verbose`)

During debugging of queue-trigger issues, logging was temporarily increased
to observe:
- queue trigger execution
- message decoding
- poison queue behavior

Once the issue was resolved, verbosity was reverted to avoid noisy logs.

---

## Default (normal development) configuration

### `host.json` (recommended default)

```json
{
  "version": "2.0",
  "logging": {
    "logLevel": {
      "default": "Information",
      "Function": "Information",
      "Host.Results": "Information",
      "Host.Aggregator": "Information"
    }
  }
}
