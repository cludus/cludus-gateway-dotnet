namespace CludusGateway.Dtos;

public record ClientMessageDto(ClientAction Action, string Recipient, string Content)
{
}

public enum ClientAction
{
    /// <summary>
    /// The message should be sent to the recipient
    /// </summary>
    Send,
    /// <summary>
    /// The message is a simple heartbeat message, it should be recorded.
    /// </summary>
    Heartbeat
}