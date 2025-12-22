using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Infrastructure.Entities;

public class ActivityEvent
{
    public long Id { get; set; }

    public long WorkItemId { get; set; }
    public WorkItem WorkItem { get; set; } = null!;

    public ActivityEventType EventType { get; set; }

    public string Actor { get; set; } = "system";
    public DateTimeOffset Timestamp { get; set; }

    public string PayloadJson { get; set; } = "{}";
}
