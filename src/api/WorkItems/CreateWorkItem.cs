using AtlasOps.Api.Contracts;
using AtlasOps.Api.Helpers;
using AtlasOps.Infrastructure.Data;
using AtlasOps.Infrastructure.Entities;
using AtlasOps.Infrastructure.Enums;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace AtlasOps.Api.WorkItems;

public sealed class CreateWorkItem
{
    private readonly AtlasOpsDbContext _db;

    public CreateWorkItem(AtlasOpsDbContext db) => _db = db;

    [Function("create-workitem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workitems")] HttpRequestData req)
    {
        var body = await HttpJson.ReadJsonAsync<CreateWorkItemRequest>(req);
        if (body is null)
            return await HttpJson.BadRequestAsync(req, "Invalid JSON body.");

        if (body.WorkspaceId <= 0)
            return await HttpJson.BadRequestAsync(req, "WorkspaceId must be > 0.");

        if (string.IsNullOrWhiteSpace(body.Title))
            return await HttpJson.BadRequestAsync(req, "Title is required.");

        var workspaceExists = await _db.Workspaces.AnyAsync(w => w.Id == body.WorkspaceId);
        if (!workspaceExists)
            return await HttpJson.BadRequestAsync(req, "Workspace does not exist.");

        if (!Enum.IsDefined(typeof(WorkItemPriority), body.Priority))
            return await HttpJson.BadRequestAsync(req, "Priority must be 1 (Low), 2 (Medium), or 3 (High).");

        var now = DateTimeOffset.UtcNow;

        var workItem = new WorkItem
        {
            WorkspaceId = body.WorkspaceId,
            Title = body.Title.Trim(),
            Description = body.Description?.Trim(),
            Status = WorkItemStatus.Submitted,
            Priority = (WorkItemPriority)body.Priority,
            CreatedBy = "anonymous",
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.WorkItems.Add(workItem);
        await _db.SaveChangesAsync();

        // Audit event
        _db.ActivityEvents.Add(new ActivityEvent
        {
            WorkItemId = workItem.Id,
            EventType = ActivityEventType.WorkItemCreated,
            Actor = "anonymous",
            Timestamp = now,
            PayloadJson = $"{{\"title\":\"{EscapeJson(workItem.Title)}\"}}"
        });

        await _db.SaveChangesAsync();

        return await HttpJson.CreatedAsync(req, new
        {
            workItem.Id,
            workItem.WorkspaceId,
            workItem.Title,
            workItem.Description,
            Status = workItem.Status.ToString(),
            Priority = workItem.Priority.ToString(),
            workItem.CreatedAt,
            workItem.UpdatedAt
        });
    }

    private static string EscapeJson(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
