namespace CludusGateway.Dtos;

public record ClientMessageDto(ClientAction Action, String Recipient, String Content)
{
}

public enum ClientAction
{
    /**
     * The message should be sent to the recipient
     */
    SEND,
    /**
     * The message is a simple heartbeat message, it should be recorded.
     */
    HEARTBEAT
}