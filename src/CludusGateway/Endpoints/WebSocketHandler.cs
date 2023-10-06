using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using CludusGateway.Dtos;
using CludusGateway.Services;
using Newtonsoft.Json;

namespace CludusGateway.Endpoints;

public class WebSocketHandler
{
    private ILogger<WebSocketHandler> _logger;

    private UserSessionRegistry _registry;

    public WebSocketHandler(ILogger<WebSocketHandler> logger, UserSessionRegistry registry)
    {
        _logger = logger;
        _registry = registry;
    }

    public async Task HandleAsync(WebSocket webSocket, ClaimsPrincipal principal)
    {
        var data = new byte[1024 * 4];
        var buffer = new ArraySegment<byte>(data);

        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        while (!result.CloseStatus.HasValue)
        {
            string message = Encoding.ASCII.GetString(data, 0, result.Count);
            _registry.GetSession(principal).MessageReceived(message);
            result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        }

        _registry.SessionClosed(webSocket, principal, result);
    }
}
