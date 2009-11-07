using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Message
{
    public interface IMessageProvider
    {
        IList<IMessage> AllMessages { get; }
        void Update();
        void RegisterForUpdates(IMessageReceiver receiver);
        IMessage MessageByID(string providerID);
        void Notify(IEnumerable<IMessage> parsedMessages);
    }

    public interface IMessageReceiver 
    {
        void Notify(IEnumerable<IMessage> updatedMessages);
    }
}
