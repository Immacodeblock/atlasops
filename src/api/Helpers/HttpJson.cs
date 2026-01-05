using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace AtlasOps.Api.Helpers;

public static class HttpJson
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public static async Task<T?> ReadJsonAsync<T>(HttpRequestData req)
    {
        return await JsonSerializer.DeserializeAsync<T>(req.Body, JsonOptions);
    }

    public static async Task<HttpResponseData> OkAsync(
        HttpRequestData req,
        object body)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        await WriteJsonAsync(res, body);
        return res;
    }

    public static async Task<HttpResponseData> CreatedAsync(
        HttpRequestData req,
        object body)
    {
        var res = req.CreateResponse(HttpStatusCode.Created);
        await WriteJsonAsync(res, body);
        return res;
    }

    public static async Task<HttpResponseData> BadRequestAsync(
        HttpRequestData req,
        string message)
    {
        var res = req.CreateResponse(HttpStatusCode.BadRequest);
        await WriteJsonAsync(res, new { error = message });
        return res;
    }

    private static async Task WriteJsonAsync(
        HttpResponseData res,
        object body)
    {
        res.Headers.Add("Content-Type", "application/json");

        var json = JsonSerializer.Serialize(body, JsonOptions);
        await res.WriteStringAsync(json);
    }
}
