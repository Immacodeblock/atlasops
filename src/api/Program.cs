using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AtlasOps.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddDbContext<AtlasOpsDbContext>(options =>
{
    var cs = Environment.GetEnvironmentVariable("SqlConnectionString")
             ?? throw new InvalidOperationException("Missing SqlConnectionString");

    options.UseSqlServer(cs, sql =>
{
    sql.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null
    );
});

});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
