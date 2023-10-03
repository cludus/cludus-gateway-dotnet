using CludusGateway.Endpoints;
using Prometheus;
using Serilog;
using System.Net.WebSockets;

namespace CludusGateway.Services;

public class UserSessionRegistry
{
    private ILogger<UserSessionRegistry> logger;

    private Dictionary<string, WebSocket> sessions = new Dictionary<string, WebSocket>();

    public UserSessionRegistry(ILogger<UserSessionRegistry> logger)
    {
        this.logger = logger;
    }
    /*
    public UserSessionHandler Register(WebSocket session)
    {
        this.logger.LogInformation("registering a new websocket connection {}", UserSessionHandler.FindUser(session));
        var result = new UserSessionHandler(session, this);
        sessions.Put(result.getUser(), result);
        updateMetrics();
        return result;
    }

    public UserSessionHandler getSession(String user)
    {
        return sessionsMap.get(user);
    }

    public UserSessionHandler getSession(WebSocketSession session)
    {
        return getSession(UserSessionHandler.findUser(session));
    }

    public void sessionClosed(WebSocketSession session, CloseStatus status)
    {
        var userSession = getSession(session);
        if (session.isOpen())
        {
            try
            {
                LOG.info("closing a websocket connection {}", UserSessionHandler.findUser(session));
                session.close(status);
                sessionsMap.remove(userSession.getUser());
            }
            catch (Exception ex)
            {
                LOG.error(ex.getMessage(), ex);
            }
        }
    }

    public void evit()
    {
        sessionsMap.values().stream()
                .filter(UserSessionHandler::isIdle)
                .forEach(UserSessionHandler::closeSession);
        List<String> toRemove = sessionsMap.values().stream()
                .filter(x-> !x.isOpen())
                .map(UserSessionHandler::getUser)
                .toList();
        LOG.info("evicting  {} websocket connections", toRemove.size());
        toRemove.forEach(k->sessionsMap.remove(k));
        updateMetrics();
    }

    private void updateMetrics()
    {
        Metrics.gauge("cludus_gateway_connections_count", sessionsMap.size());
    }*/
}
