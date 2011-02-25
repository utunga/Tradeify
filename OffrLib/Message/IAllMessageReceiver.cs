namespace Offr.Message
{
    //FIXME this is a giant hack
    public interface IAllMessageReceiver
    {
        void Push(IMessage parsedMessage);
    }
}