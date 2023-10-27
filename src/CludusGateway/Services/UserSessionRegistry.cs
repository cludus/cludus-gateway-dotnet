using Prometheus;
using System.Net.WebSockets;
using System.Security.Claims;

namespace CludusGateway.Services;

public class UserSessionRegistry
{
    private ILogger<UserSessionRegistry> _logger;

    private Dictionary<string, UserSessionHandler> _sessions = new Dictionary<string, UserSessionHandler>();
    
    private readonly Gauge _msgCount;

    public UserSessionRegistry(ILogger<UserSessionRegistry> logger)
    {
        _logger = logger;
        _msgCount = Metrics.CreateGauge("cludus_gateway_connections_count", "# of user connections received at Cludus Gateway");
    }

    public UserSessionHandler Register(WebSocket session, ClaimsPrincipal principal)
    {
        _logger.LogInformation("registering a new websocket connection {}", UserSessionHandler.FindUser(principal));

        var result = new UserSessionHandler(session, principal, this);
        _sessions.Add(result.User, result);

        UpdateMetrics();
        
        return result;
    }

    public UserSessionHandler GetSession(string user)
    {
        return _sessions[user];
    }

    public UserSessionHandler GetSession(ClaimsPrincipal principal)
    {
        return GetSession(UserSessionHandler.FindUser(principal));
    }

    public void SessionClosed(WebSocket session, ClaimsPrincipal principal, WebSocketReceiveResult result)
    {
        var userSession = GetSession(principal);
        if (session.State == WebSocketState.Open)
        {
            session.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None).GetAwaiter().GetResult();
            _sessions.Remove(userSession.User);
        }
    }
    
    public void Evit()
    {
        var idles = _sessions.Values.Where(x => x.Idle).ToList();
        foreach (var s in idles)
        {
            s.CloseSession();
        }

        var closed = _sessions.Values.Where(x => !x.Open).Select(x => x.User);
        foreach (var user in closed)
        {
            _sessions.Remove(user);
        }

        UpdateMetrics();
    }

    private void UpdateMetrics()
    {
        _msgCount.Set(_sessions.Count);
    }
}
