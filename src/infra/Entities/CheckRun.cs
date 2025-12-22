using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Infrastructure.Entities;

public class CheckRun
{
    public long Id { get; set; }

    public long WorkItemId { get; set; }
    public WorkItem WorkItem { get; set; } = null!;

    public string CheckType { get; set; } = null!;
    public CheckStatus Status { get; set; }

    public string? OutputSummary { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExecutedAt { get; set; }
}
