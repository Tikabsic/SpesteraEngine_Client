using Google.Protobuf;
public interface IMessageHandler<T> where T : IMessage
{
    public void HandleMessage(T Message);
}
