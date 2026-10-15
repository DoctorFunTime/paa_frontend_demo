var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();
builder.Services.AddHttpClient("development-api", client =>
{
    var gateway = builder.Configuration["ApiSettings:BaseUrl"];
    if (!Uri.TryCreate(gateway, UriKind.Absolute, out var address) || address.Scheme != Uri.UriSchemeHttps)
        throw new InvalidOperationException("ApiSettings:BaseUrl must be an HTTPS development gateway URL.");
    client.BaseAddress = new Uri(address.GetLeftPart(UriPartial.Authority));
    client.Timeout = TimeSpan.FromSeconds(115);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect = false,
    UseCookies = false
});

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.UseWebAssemblyDebugging();

// Only the known API path is forwarded. The client authenticates with the same
// development gateway as the main frontend; this host stores no school records.
app.MapMethods("/api/v1/{**path}", ["GET", "POST", "PUT", "PATCH", "DELETE"], async (
    HttpContext context, IHttpClientFactory clients, string? path) =>
{
    if ((path ?? "").Split('/').Any(segment => Uri.UnescapeDataString(segment) is "." or ".."))
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }
    var client = clients.CreateClient("development-api");
    var relativePath = "/api/v1/" + (path ?? "");
    var target = new Uri(client.BaseAddress!, relativePath + context.Request.QueryString);
    using var outbound = new HttpRequestMessage(new HttpMethod(context.Request.Method), target);
    if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
        outbound.Content = new StreamContent(context.Request.Body);

    foreach (var header in context.Request.Headers)
    {
        if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Origin", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Referer", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("Upgrade", StringComparison.OrdinalIgnoreCase) ||
            header.Key.Equals("TE", StringComparison.OrdinalIgnoreCase)) continue;
        if (!outbound.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            outbound.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
    }

    try
    {
        using var response = await client.SendAsync(outbound, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        context.Response.StatusCode = (int)response.StatusCode;
        foreach (var header in response.Headers.Concat(response.Content.Headers))
        {
            if (header.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Connection", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Access-Control-Allow-Origin", StringComparison.OrdinalIgnoreCase) ||
                header.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase)) continue;
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }
        context.Response.Headers.CacheControl = "no-store";
        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
    catch (OperationCanceledException) when (!context.RequestAborted.IsCancellationRequested)
    {
        context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
    }
    catch (HttpRequestException)
    {
        context.Response.StatusCode = StatusCodes.Status502BadGateway;
    }
});

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.Run();
