using AtlasOps.Api.Contracts.WorkItems;
using AtlasOps.Api.Helpers;
using AtlasOps.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace AtlasOps.Api.WorkItems;

public sealed class GetWorkItemTimeline
{
    private readonly AtlasOpsDbContext _db;

    public GetWorkItemTimeline(AtlasOpsDbContext db)
    {
        _db = db;
    }

    [Function("get-workitem-timeline")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workitems/{id:long}/timeline")]
        HttpRequestData req,
        long id)
    {
        // Ensure work item exists
        var exists = await _db.WorkItems
            .AsNoTracking()
            .AnyAsync(w => w.Id == id);

        if (!exists)
            return await HttpJson.NotFoundAsync(req, $"WorkItem {id} not found");

        var events = await _db.ActivityEvents
            .AsNoTracking()
            .Where(e => e.WorkItemId == id)
            .OrderBy(e => e.Timestamp)
            .Select(e => new WorkItemTimelineEventDto(
                e.Id,
                e.EventType,
                e.Actor,
                e.PayloadJson,
                e.Timestamp
            ))
            .ToListAsync();

        return await HttpJson.OkAsync(req, events);
    }
}
