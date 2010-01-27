using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    public interface IMessageQueryExecutor
    {
        TagCounts GetAllTagCounts();
        MessagesWithTagCounts GetMessagesWithTagCounts(IEnumerable<ITag> tags);
        IEnumerable<IMessage> GetMessagesForTags(IEnumerable<ITag> tags);
        IEnumerable<IMessage> GetMessagesForTagsCreatedByUser(IEnumerable<ITag> tags, IUserPointer userPointer);
        IEnumerable<IMessage> GetMessagesCreatedByUser(IUserPointer userPointer);
    }
}
