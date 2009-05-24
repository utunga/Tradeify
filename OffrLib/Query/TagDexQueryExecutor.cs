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
        private readonly TagDex _globalTagIndex;
        public TagDexQueryExecutor(TagDex globalTagIndex)
        {
           _globalTagIndex = globalTagIndex; //FIXME should be provided by the Kernel as a singleton i guess?
        }

        public IEnumerable<IMessage> GetMessagesForQuery(IMessageQuery query)
        {
            TagDex tagDex;
            if (query.Keywords == null)
            {
                // we can use the cached data
               tagDex = _globalTagIndex;
            }
            else
            {
                // theoretically if we were caching all these objects this provider would update every time
                // new messages were published against this keyword
                MessageProviderForKeywords keywordMessageProvider = Global.Kernel.Get<MessageProviderForKeywords>(With.Parameters.ConstructorArgument("keywords", query.Keywords));
                tagDex = new TagDex(keywordMessageProvider);
            }

            return tagDex.MessagesForTags(query.Facets);
        }

        public TagCounts GetTagCountsForQuery(IMessageQuery query)
        {
            IEnumerable<IMessage> messages = GetMessagesForQuery(query);
            TagDex tagDex = new TagDex(messages);
            return tagDex.GetTagCounts();
        }
    }
}
