using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Api.Contracts.WorkItems;

public sealed record WorkItemDetailDto(
    long Id,
    int WorkspaceId,
    string Title,
    string? Description,
    WorkItemStatus Status,
    WorkItemPriority Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
