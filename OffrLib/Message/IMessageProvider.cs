using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Message
{
    public interface IMessageProvider
    {
        IList<IMessage> AllMessages { get; }
        void Notify(IEnumerable<IMessage> parsedMessages);
    }
}
