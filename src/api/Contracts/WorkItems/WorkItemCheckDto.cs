using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Api.Contracts.WorkItems;

public sealed record WorkItemCheckDto(
    long Id,
    string CheckType,
    CheckStatus Status,
    string? OutputSummary,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExecutedAt
);
