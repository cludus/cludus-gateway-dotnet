using CludusGateway.Endpoints;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;
using Serilog.Debugging;

SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .WriteTo.Console();
});

builder.Services.AddHealthChecks()
    .AddCheck("gateway-dotnet", () => HealthCheckResult.Healthy())
    .ForwardToPrometheus();

builder.Services.AddScoped<WebSocketHandler>();

var app = builder.Build();

// <snippet_UseWebSockets>
var webSocketOptions = new WebSocketOptions
{
    //KeepAliveInterval = TimeSpan.FromMinutes(2)
};
// </snippet_UseWebSockets>

app.UseWebSockets(webSocketOptions);

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


app.UseRouting().UseAuthentication().UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
    endpoints.Map("websocket", async context =>
    {
        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            if (webSocket is not null)
            {
                await webSocketHandler.HandleAsync(webSocket, context.User);
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }).RequireAuthorization();
});

app.Run();