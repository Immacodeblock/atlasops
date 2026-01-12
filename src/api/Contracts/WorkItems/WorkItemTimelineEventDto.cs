using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Api.Contracts.WorkItems;

public sealed record WorkItemTimelineEventDto(
    long Id,
    ActivityEventType EventType,
    string Actor,
    string? PayloadJson,
    DateTimeOffset Timestamp
);
