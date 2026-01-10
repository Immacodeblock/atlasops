using System.Net;
using AtlasOps.Api.Contracts.WorkItems;
using AtlasOps.Api.Helpers;
using AtlasOps.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace AtlasOps.Api.WorkItems;

public sealed class GetWorkItem
{
    private readonly AtlasOpsDbContext _db;

    public GetWorkItem(AtlasOpsDbContext db)
    {
        _db = db;
    }

    [Function("get-workitem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workitems/{id:long}")] HttpRequestData req,
        long id)
    {
        var wi = await _db.WorkItems
            .AsNoTracking()
            .Where(w => w.Id == id)
            .Select(w => new WorkItemDetailDto(
                w.Id,
                w.WorkspaceId,
                w.Title,
                w.Description,
                w.Status,
                w.Priority,
                w.CreatedAt,
                w.UpdatedAt
            ))
            .FirstOrDefaultAsync();

        if (wi is null)
            return await HttpJson.NotFoundAsync(req, $"WorkItem {id} not found");

        return await HttpJson.OkAsync(req, wi);
    }
}
