using AtlasOps.Infrastructure.Data;
using AtlasOps.Infrastructure.Entities;
using AtlasOps.Infrastructure.Enums;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AtlasOps.Api.Checks;




public sealed class RunChecks
{
    private readonly AtlasOpsDbContext _db;
    private readonly ILogger<RunChecks> _logger;

    public RunChecks(AtlasOpsDbContext db, ILogger<RunChecks> logger)
    {
        _db = db;
        _logger = logger;
    }

    [Function("run-checks")]
    public async Task Run(
        [QueueTrigger("workitem-checks", Connection = "AzureWebJobsStorage")] string message)
    {
        _logger.LogInformation("Received queue message: {Message}", message);

    try
        {

        var payload = JsonSerializer.Deserialize<QueuePayload>(
            message,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (payload is null || payload.WorkItemId <= 0)
        {
            _logger.LogWarning("Invalid queue payload");
            return;
        }

        var workItem = await _db.WorkItems.FindAsync(payload.WorkItemId);
        if (workItem is null)
        {
            _logger.LogWarning("WorkItem {Id} not found", payload.WorkItemId);
            return;
        }

        var now = DateTimeOffset.UtcNow;

        // Dummy checks (simulate real ones later)
        var checks = new[]
        {
            new CheckRun
            {
                WorkItemId = workItem.Id,
                CheckType = "SchemaValidation",
                Status = CheckStatus.Pass,
                OutputSummary = "Required fields present",
                CreatedAt = now,
                ExecutedAt = now
            },
            new CheckRun
            {
                WorkItemId = workItem.Id,
                CheckType = "SecurityScan",
                Status = CheckStatus.Warning,
                OutputSummary = "No critical issues found",
                CreatedAt = now,
                ExecutedAt = now
            }
        };

        _db.CheckRuns.AddRange(checks);

        _db.ActivityEvents.Add(new ActivityEvent
        {
            WorkItemId = workItem.Id,
            EventType = ActivityEventType.ChecksCompleted,
            Actor = "system",
            Timestamp = now,
            PayloadJson = "{\"checks\":2}"
        });

        // Optional state move
        workItem.Status = WorkItemStatus.InReview;
        workItem.UpdatedAt = now;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Checks completed for WorkItem {Id}", workItem.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WorkItem checks");
        }
    }


    private sealed record QueuePayload(long WorkItemId);
}
