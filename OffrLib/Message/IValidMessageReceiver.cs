namespace Offr.Message
{
    public interface IValidMessageReceiver
    {
        void Push(IMessage parsedMessage);
    }
}