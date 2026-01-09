using System.Net;
using AtlasOps.Api.Contracts;
using AtlasOps.Api.Contracts.WorkItems;
using AtlasOps.Api.Helpers;
using AtlasOps.Infrastructure.Data;
using AtlasOps.Infrastructure.Enums;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AtlasOps.Api.WorkItems;

public sealed class ListWorkItems
{
    private readonly AtlasOpsDbContext _db;
    private readonly ILogger<ListWorkItems> _logger;

    public ListWorkItems(AtlasOpsDbContext db, ILogger<ListWorkItems> logger)
    {
        _db = db;
        _logger = logger;
    }

    [Function("list-workitems")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workitems")] HttpRequestData req)
    {
        // Query params
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

        var page = TryInt(query["page"], 1);
        var pageSize = TryInt(query["pageSize"], 20);

        // Guardrails
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        int? workspaceId = TryNullableInt(query["workspaceId"]);

        WorkItemStatus? status = TryEnum<WorkItemStatus>(query["status"]);
        // optional later: priority filter, search, etc.

        // Base query (NO tracking for reads)
        IQueryable<Infrastructure.Entities.WorkItem> q = _db.WorkItems.AsNoTracking();

        if (workspaceId is not null)
            q = q.Where(wi => wi.WorkspaceId == workspaceId.Value);

        if (status is not null)
            q = q.Where(wi => wi.Status == status.Value);

        // Stable ordering (important for pagination)
        q = q.OrderByDescending(wi => wi.CreatedAt).ThenByDescending(wi => wi.Id);

        var totalCount = await q.CountAsync();

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(wi => new WorkItemSummaryDto(
                wi.Id,
                wi.WorkspaceId,
                wi.Title,
                wi.Status,
                wi.Priority,
                wi.CreatedAt,
                wi.UpdatedAt
            ))
            .ToListAsync();

        var result = new PagedResult<WorkItemSummaryDto>(items, page, pageSize, totalCount);

        return await HttpJson.OkAsync(req, result);
    }

    private static int TryInt(string? value, int fallback)
        => int.TryParse(value, out var v) ? v : fallback;

    private static int? TryNullableInt(string? value)
        => int.TryParse(value, out var v) ? v : null;

    private static TEnum? TryEnum<TEnum>(string? value) where TEnum : struct
        => Enum.TryParse<TEnum>(value, ignoreCase: true, out var v) ? v : null;
}
