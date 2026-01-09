using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Api.Contracts.WorkItems;

public sealed record WorkItemSummaryDto(
    long Id,
    int WorkspaceId,
    string Title,
    WorkItemStatus Status,
    WorkItemPriority Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
