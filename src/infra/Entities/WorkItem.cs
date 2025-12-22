using AtlasOps.Infrastructure.Enums;

namespace AtlasOps.Infrastructure.Entities;

public class WorkItem
{
    public long Id { get; set; }

    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public WorkItemStatus Status { get; set; }
    public WorkItemPriority Priority { get; set; }

    public string CreatedBy { get; set; } = "system";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<CheckRun> CheckRuns { get; set; } = new List<CheckRun>();
    public ICollection<ActivityEvent> ActivityEvents { get; set; } = new List<ActivityEvent>();
}
