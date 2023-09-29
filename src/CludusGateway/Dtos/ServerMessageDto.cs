namespace CludusGateway.Dtos;

public class ServerMessageDto    
{
    
    public Actions Action { get; private set; }

    public string ErrorMsg { get; private set; }
    public string Sender { get; private set; }
    public string Content { get; private set; }

    public static ServerMessageDto Ack()
    {
        ServerMessageDto result = new ServerMessageDto();
        result.Action = Actions.ACK;
        return result;
    }

    public static ServerMessageDto Error(String errorMsg)
    {
        ServerMessageDto result = new ServerMessageDto();
        result.Action = Actions.ERROR;
        result.ErrorMsg = errorMsg;
        return result;
    }

    public static ServerMessageDto Message(String sender, String content)
    {
        ServerMessageDto result = new ServerMessageDto();
        result.Action = Actions.MESSAGE;
        result.Sender = sender;
        result.Content = content;
        return result;
    }
}

public enum Actions
{
    /**
     * The message has been received
     */
    ACK,
    ERROR,
    MESSAGE
}