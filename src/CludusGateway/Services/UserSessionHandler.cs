using CludusGateway.Dtos;
using System.Net.WebSockets;

namespace CludusGateway.Services;

public class UserSessionHandler
{
    private ILogger<UserSessionHandler> logger;

    private WebSocket session;

    public UserSessionHandler(ILogger<UserSessionHandler> logger)
    {
        this.logger = logger;
    }
    /*
    public static String FindUser(WebSocket session)
    {
        session.
        return Objects.requireNonNull(session.getPrincipal()).getName();
    }

    public UserSessionHandler(WebSocketSession session, UserSessionRegistry registry)
    {
        this.session = session;
        this.registry = registry;
        this.user = findUser(session);
        this.lastUpdated = LocalDateTime.now();
    }

    public boolean isIdle()
    {
        return session.isOpen() && lastUpdated.isBefore(LocalDateTime.now().minusMinutes(2));
    }

    public boolean isOpen()
    {
        return session.isOpen();
    }

    public void closeSession()
    {
        try
        {
            session.close(CloseStatus.NO_CLOSE_FRAME);
        }
        catch (Exception ex)
        {
            LOG.error(ex.getMessage(), ex);
        }
    }

    public void messageReceived(TextMessage message)
    {
        try
        {
            var clientMsg = readClientMessage(message);
            if (clientMsg.getAction() == ClientMessageDto.Actions.SEND)
            {
                sendActionReceived(clientMsg);
            }
            else if (clientMsg.getAction() == ClientMessageDto.Actions.HEARTBEAT)
            {
                heartBeatReceived(clientMsg);
            }
            var response = ServerMessageDto.ack();
            sendMessage(toTextMessage(response));
        }
        catch (JsonSyntaxException ex)
        {
            var response = ServerMessageDto.error(ex.getMessage());
            sendMessage(response);
        }
        catch (Exception ex)
        {
            LOG.error(ex.getMessage(), ex);
            var response = ServerMessageDto.error(ex.getMessage());
            sendMessage(toTextMessage(response));
        }
    }

    void heartBeatReceived(ClientMessageDto clientMsg)
    {
        lastUpdated = LocalDateTime.now();
    }

    void sendActionReceived(ClientMessageDto clientMsg) throws IOException
    {
        lastUpdated = LocalDateTime.now();
        var response = ServerMessageDto.message(user, clientMsg.getContent());
    var reciptHandler = registry.getSession(clientMsg.getRecipient());
        if(reciptHandler != null) {
            reciptHandler.sendMessage(toTextMessage(response));
        }
        else {
            LOG.warn("User {} is not connected.", clientMsg.getRecipient());
        }
    }

    private ClientMessageDto readClientMessage(TextMessage message)
{
    return GSON.fromJson(message.getPayload(), ClientMessageDto.class);
    }

    private TextMessage toTextMessage(ServerMessageDto message)
{
    return new TextMessage(GSON.toJson(message));
}

synchronized void sendMessage(TextMessage message) {
    try
    {
        session.sendMessage(message);
    }
    catch (Exception ex)
    {
        LOG.error(ex.getMessage(), ex);
    }
}

void sendMessage(ServerMessageDto message)
{
    sendMessage(toTextMessage(message));
}*/
}
