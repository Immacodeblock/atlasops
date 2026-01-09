# Kudu / Runtime Diagnostics (Azure Functions)

This runbook documents the use of **Kudu (SCM / Advanced Tools)** for
diagnosing Azure Function App deployments and runtime behavior.

---

## What is Kudu?

Kudu is an internal diagnostics and management site exposed by
Azure App Services and Azure Function Apps.

It provides:
- Direct access to the deployed filesystem
- Runtime logs
- Ad-hoc command execution
- Deployment verification

Kudu shows **what is actually running**, independent of pipelines or portal UI.

---

## How to access

Azure Portal → Function App → Advanced Tools → Go

This opens:

https://<function-app-name>.scm.azurewebsites.net


---

## Key areas in Kudu

### Debug Console
- CMD (Windows)
- Bash (Linux)

Used to:
- navigate deployed files
- inspect directories
- verify deployment contents

---

### Deployed filesystem

Navigate to:


---

## Key areas in Kudu

### Debug Console
- CMD (Windows)
- Bash (Linux)

Used to:
- navigate deployed files
- inspect directories
- verify deployment contents

---

### Deployed filesystem

Navigate to:

Debug Console → CMD → site/wwwroot


Expected contents for a .NET isolated Functions app:
- `<AppName>.dll`
- `<AppName>.deps.json`
- `<AppName>.runtimeconfig.json`
- `host.json`
- `functions.metadata`
- `worker.config.json`
- `runtimes/`

This was used during AtlasOps setup to:
- confirm GitHub Actions actually deployed files
- verify app binaries existed even when HTTP requests failed

---

### Logs

Kudu exposes log directories such as:
- `LogFiles/`
- `Application/Functions/Host/`
- Deployment logs

Useful when:
- portal log stream is delayed
- runtime behaves inconsistently
- Function App appears deployed but not responsive

---

## When to use Kudu

Use Kudu when:
- deployment reports success but app returns 500
- Function App “suddenly starts working”
- portal UI contradicts observed behavior
- you need to verify runtime state directly

Do NOT use Kudu to:
- edit files
- make persistent changes
- bypass deployment pipelines

---

## Key lesson

When debugging Azure Functions:
- Trust **Kudu over dashboards**
- Trust **deployed files over pipelines**
- Trust **runtime evidence over assumptions**
