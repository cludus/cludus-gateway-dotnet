using System.Net.WebSockets;
using System.Security.Claims;

namespace CludusGateway.Endpoints;

public class WebSocketHandler
{
    private ILogger<WebSocketHandler> logger;

    public static int COUNT;

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        this.logger = logger;
    }

    public async Task HandleAsync(WebSocket webSocket, ClaimsPrincipal principal)
    {
        var buffer = new byte[1024 * 4];

        Interlocked.Increment(ref COUNT);
        logger.LogInformation($"Connection #{COUNT} accepted.");
                
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        Interlocked.Decrement(ref COUNT);

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);

        logger.LogInformation($"Connection #{COUNT} closed.");
    }
}
