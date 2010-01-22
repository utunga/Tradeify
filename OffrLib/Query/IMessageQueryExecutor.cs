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
        TagCounts GetTagCounts();
        TagCounts GetTagCountsForTags(IEnumerable<ITag> tags);
        IEnumerable<IMessage> GetMessagesForTags(IEnumerable<ITag> tags,bool includeIgnoredUsers);
        //IEnumerable<IMessage> GetMessagesForKeywordAndTags(string keyword, IEnumerable<ITag> tags);
        IEnumerable<IMessage> GetMessagesForTagsCreatedByUser(IEnumerable<ITag> tags, IUserPointer userPointer);
        IEnumerable<IMessage> GetMessagesCreatedByUser(IUserPointer userPointer);
    }
}
