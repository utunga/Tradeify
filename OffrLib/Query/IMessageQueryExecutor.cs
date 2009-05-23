using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;

namespace Offr.Query
{
    public interface IMessageQueryExecutor
    {
        IEnumerable<IMessage> GetMessagesForQuery(IMessageQuery messageQuery);
    }
}
