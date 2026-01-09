namespace AtlasOps.Api.Contracts;

public sealed record CreateWorkItemRequest(
    int WorkspaceId,
    string Title,
    string? Description,
    int Priority // 1=Low, 2=Medium, 3=High (maps to enum)
);
