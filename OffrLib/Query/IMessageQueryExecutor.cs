using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    public interface IMessageQueryExecutor
    {
        IEnumerable<IMessage> GetMessagesForQuery(IMessageQuery messageQuery);
        TagCounts GetTagCountsForQuery(IMessageQuery query);
    }
}
