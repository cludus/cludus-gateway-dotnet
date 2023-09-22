using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console();
});

builder.Services.AddHealthChecks()
    .AddCheck("gateway-dotnet", () => HealthCheckResult.Healthy())
    .ForwardToPrometheus();

var app = builder.Build();

// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    //KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
// </snippet_UseWebSockets>

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSerilogRequestLogging();

app.MapHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// This will create metrics for each HTTP endpoint, giving us 'number of requests', 'request times' etc 
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("host", context => context.Request.Host.Host);
    options.AddCustomLabel("logicalService", context => "gateway-dotnet");
});

app.UseRouting().UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();

    endpoints.Map("websocket", async (context) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            await CludusGateway.Helpers.EchoHelper.Echo(webSocket, logger);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    });
});

app.Run();