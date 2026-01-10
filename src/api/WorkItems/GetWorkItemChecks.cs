using AtlasOps.Api.Contracts.WorkItems;
using AtlasOps.Api.Helpers;
using AtlasOps.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace AtlasOps.Api.WorkItems;

public sealed class GetWorkItemChecks
{
    private readonly AtlasOpsDbContext _db;

    public GetWorkItemChecks(AtlasOpsDbContext db)
    {
        _db = db;
    }

    [Function("get-workitem-checks")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workitems/{id:long}/checks")]
        HttpRequestData req,
        long id)
    {
        // Ensure work item exists (clear 404 vs empty list)
        var exists = await _db.WorkItems
            .AsNoTracking()
            .AnyAsync(w => w.Id == id);

        if (!exists)
            return await HttpJson.NotFoundAsync(req, $"WorkItem {id} not found");

        var checks = await _db.CheckRuns
            .AsNoTracking()
            .Where(c => c.WorkItemId == id)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new WorkItemCheckDto(
                c.Id,
                c.CheckType,
                c.Status,
                c.OutputSummary,
                c.CreatedAt,
                c.ExecutedAt
            ))
            .ToListAsync();

        return await HttpJson.OkAsync(req, checks);
    }
}
