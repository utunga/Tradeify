using System.Collections.Generic;

namespace Offr.Message
{
    public interface IMessageReceiver 
    {
        void Notify(IEnumerable<IMessage> updatedMessages);
    }
}