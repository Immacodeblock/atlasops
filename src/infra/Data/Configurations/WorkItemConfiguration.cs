using AtlasOps.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasOps.Infrastructure.Data.Configurations;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("WorkItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Priority).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasOne(x => x.Workspace)
            .WithMany(w => w.WorkItems)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Query-driven indexes
        builder.HasIndex(x => new { x.WorkspaceId, x.Status, x.UpdatedAt });
        builder.HasIndex(x => new { x.WorkspaceId, x.CreatedAt });
    }
}
