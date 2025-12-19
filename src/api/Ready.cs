using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AtlasOps.Api;

public class Ready
{
    private readonly ILogger _logger;

    public Ready(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Ready>();
    }

    [Function("ready")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ready")] HttpRequestData req)
    {
        var res = req.CreateResponse();

        var connStr = Environment.GetEnvironmentVariable("SqlConnectionString");
        if (string.IsNullOrWhiteSpace(connStr))
        {
            res.StatusCode = HttpStatusCode.ServiceUnavailable;
            await res.WriteStringAsync("""{"status":"not_ready","reason":"missing SqlConnectionString"}""");
            return res;
        }

        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand("SELECT 1", conn);
            var result = await cmd.ExecuteScalarAsync();

            res.StatusCode = HttpStatusCode.OK;
            await res.WriteStringAsync($$"""{"status":"ready","db":"ok","result":{{result}}}""");
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed (SQL).");
            res.StatusCode = HttpStatusCode.ServiceUnavailable;
            await res.WriteStringAsync("""{"status":"not_ready","db":"error"}""");
            return res;
        }
    }
}
