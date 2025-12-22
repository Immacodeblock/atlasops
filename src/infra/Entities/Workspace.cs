namespace AtlasOps.Infrastructure.Entities;

public class Workspace
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
}
