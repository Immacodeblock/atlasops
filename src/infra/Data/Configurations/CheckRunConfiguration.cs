using AtlasOps.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasOps.Infrastructure.Data.Configurations;

public class CheckRunConfiguration : IEntityTypeConfiguration<CheckRun>
{
    public void Configure(EntityTypeBuilder<CheckRun> builder)
    {
        builder.ToTable("CheckRuns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CheckType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.OutputSummary)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.WorkItem)
            .WithMany(w => w.CheckRuns)
            .HasForeignKey(x => x.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.WorkItemId, x.CreatedAt });
        builder.HasIndex(x => new { x.WorkItemId, x.ExecutedAt });
    }
}
