using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AtlasOps.Infrastructure.Data;

public class AtlasOpsDbContextFactory : IDesignTimeDbContextFactory<AtlasOpsDbContext>
{
    public AtlasOpsDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("SqlConnectionString");
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Missing SqlConnectionString for design-time EF Core.");

        var options = new DbContextOptionsBuilder<AtlasOpsDbContext>()
    .UseSqlServer(cs, sql =>
    {
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        );
    })
    .Options;


        return new AtlasOpsDbContext(options);
    }
}
