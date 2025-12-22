using AtlasOps.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtlasOps.Infrastructure.Data;

public class AtlasOpsDbContext : DbContext
{
    public AtlasOpsDbContext(DbContextOptions<AtlasOpsDbContext> options) : base(options) { }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<CheckRun> CheckRuns => Set<CheckRun>();
    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtlasOpsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
