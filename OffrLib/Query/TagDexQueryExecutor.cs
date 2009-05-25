using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core.Parameters;
using Offr.Common;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    public class TagDexQueryExecutor : IMessageQueryExecutor
    {
        private readonly MessageReceivingTagDex _globalTagIndex;

        public TagDexQueryExecutor()
        {
            IMessageProvider globalMessageProvider = Global.Kernel.Get<IMessageProvider>();
            _globalTagIndex = new MessageReceivingTagDex(globalMessageProvider);
        }

        public IEnumerable<IMessage> GetMessagesForQuery(IMessageQuery query)
        {
            MessageReceivingTagDex tagDex;
            if (query.Keywords == null)
            {
                // we can use the globally cached data, because we are not doing text search
               tagDex = _globalTagIndex;
            }
            else
            {
                // theoretically if we were caching all these objects this use of a 
                // MessageReceivingTagDex would mean that we get up dates from the feed
                // when new messages match the query, but.. since we don't cache it anyway, that
                // is kind of pointless
                MessageProviderForKeywords keywordMessageProvider = Global.Kernel.Get<MessageProviderForKeywords>(With.Parameters.ConstructorArgument("keywords", query.Keywords));
                tagDex = new MessageReceivingTagDex(keywordMessageProvider);
            }

            return tagDex.MessagesForTags(query.Facets);
        }

        public TagCounts GetTagCountsForQuery(IMessageQuery query)
        {
            IEnumerable<IMessage> messages = GetMessagesForQuery(query);
            StaticTagDex tagDex = new StaticTagDex(messages);
            return tagDex.GetTagCounts();
        }

        public TagCounts GetTagCountsForTags(List<ITag> tags)
        {
            var tagCounts = new List<TagWithCount>();
            foreach (ITag tag in tags)
            {
                tagCounts.Add(_globalTagIndex.GetTagCountForTag(tag));
            }
            //tagCounts.Reverse();
            return new TagCounts() { Tags = tagCounts, Total = -1 };
        }
    }
}
