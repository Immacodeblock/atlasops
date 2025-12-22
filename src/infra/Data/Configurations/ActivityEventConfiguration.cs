using AtlasOps.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AtlasOps.Infrastructure.Data.Configurations;

public class ActivityEventConfiguration : IEntityTypeConfiguration<ActivityEvent>
{
    public void Configure(EntityTypeBuilder<ActivityEvent> builder)
    {
        builder.ToTable("ActivityEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Actor)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PayloadJson)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.EventType)
            .IsRequired();

        builder.HasOne(x => x.WorkItem)
            .WithMany(w => w.ActivityEvents)
            .HasForeignKey(x => x.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.WorkItemId, x.Timestamp });
    }
}
