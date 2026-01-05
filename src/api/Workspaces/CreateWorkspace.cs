using AtlasOps.Api.Contracts;
using AtlasOps.Api.Helpers;
using AtlasOps.Infrastructure.Data;
using AtlasOps.Infrastructure.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;

namespace AtlasOps.Api.Workspaces;

public sealed class CreateWorkspace
{
    private readonly AtlasOpsDbContext _db;

    public CreateWorkspace(AtlasOpsDbContext db) => _db = db;

    [Function("create-workspace")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workspaces")] HttpRequestData req)
    {
        var body = await HttpJson.ReadJsonAsync<CreateWorkspaceRequest>(req);
        if (body is null || string.IsNullOrWhiteSpace(body.Name))
            return await HttpJson.BadRequestAsync(req, "Name is required.");

        var name = body.Name.Trim();

        var exists = await _db.Workspaces.AnyAsync(w => w.Name == name);
        if (exists)
            return await HttpJson.BadRequestAsync(req, "Workspace name already exists.");

        var ws = new Workspace
        {
            Name = name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Workspaces.Add(ws);
        await _db.SaveChangesAsync();

        return await HttpJson.CreatedAsync(req, new { ws.Id, ws.Name, ws.CreatedAt });
    }
}
