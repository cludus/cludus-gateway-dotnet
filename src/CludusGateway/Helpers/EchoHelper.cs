using System.Net.WebSockets;

namespace CludusGateway.Helpers;

internal static class EchoHelper
{
    public static int COUNT;

    internal static async Task Echo(WebSocket webSocket, ILogger<Program> logger)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        Interlocked.Increment(ref COUNT);
        logger.LogInformation($"Connection #{COUNT} accepted.");
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
