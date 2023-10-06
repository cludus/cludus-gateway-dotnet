using CludusGateway.Dtos;
using System.Net.WebSockets;
using System.Security.Claims;
using Newtonsoft.Json;

namespace CludusGateway.Services;

public class UserSessionHandler
{
    private WebSocket _session;

    private UserSessionRegistry _registry;

    private DateTime _lastUpdate;

    private string _user;

    public UserSessionHandler(WebSocket session, ClaimsPrincipal principal, UserSessionRegistry registry)
    {
        _session = session;
        _registry = registry;
        _lastUpdate = DateTime.Now;
        _user = FindUser(principal);
    }

    public static string FindUser(ClaimsPrincipal principal)
    {
        return principal.Identity.Name;
    }

    public string User
    {
        get => _user;
    }

    public bool Idle
    {
        get => Open && _lastUpdate < DateTime.Now.AddMinutes(-2);
    }

    public bool Open
    {
        get => _session.State == WebSocketState.Open;
    }

    public void CloseSession()
    {
        _session.CloseAsync(WebSocketCloseStatus.NormalClosure, "status", CancellationToken.None).GetAwaiter().GetResult();
    }

    public void MessageReceived(string message)
    {
        try
        {
            var clientMsg = ReadClientMessage(message);
            if (clientMsg.Action == ClientAction.Send)
            {
                SendActionReceived(clientMsg);
            }
            else if (clientMsg.Action == ClientAction.Heartbeat)
            {
                HeartBeatReceived(clientMsg);
            }
            var response = ServerMessageDto.Ack();
            SendMessage(ToTextMessage(response));
        }
        catch (Exception ex)
        {
            var response = ServerMessageDto.Error(ex.Message);
            SendMessage(response);
        }
    }

    void HeartBeatReceived(ClientMessageDto clientMsg)
    {
        _lastUpdate = DateTime.Now;
    }

    void SendActionReceived(ClientMessageDto clientMsg)
    {
        _lastUpdate = DateTime.Now;
        var response = ServerMessageDto.Message(_user, clientMsg.Content);
        var reciptHandler = _registry.GetSession(clientMsg.Recipient);
        reciptHandler.SendMessage(ToTextMessage(response));
    }

    private ClientMessageDto ReadClientMessage(string message)
    {
        return JsonConvert.DeserializeObject<ClientMessageDto>(message);
    }

    private string ToTextMessage(ServerMessageDto dto)
    {
        return JsonConvert.SerializeObject(dto);
    }
    
    ValueTask SendMessage(string message) {
        byte[] data = System.Text.Encoding.UTF8.GetBytes (message);
        var buffer = new ReadOnlyMemory<byte>(data);
        return _session.SendAsync(buffer, 
            WebSocketMessageType.Text, 
            WebSocketMessageFlags.EndOfMessage, 
            CancellationToken.None);
    }

    void SendMessage(ServerMessageDto message)
    {
        SendMessage(ToTextMessage(message));
    }
}
