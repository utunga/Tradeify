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
       
        private TagDex _globalTagIndex;

        public TagDexQueryExecutor(TagDex globalTagIndex)
        {
           _globalTagIndex = globalTagIndex;
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
                //throw new NotImplementedException("Not implemented yet");
                //// we can't use the cached data FIXME(This kinda sucks)
                MessageProviderForKeywords keywordProvider = Global.Kernel.Get<MessageProviderForKeywords>(With.Parameters.ConstructorArgument("keywords", query.Keywords));
                tagDex = new TagDex(keywordProvider, Global.Kernel.Get<ITagProvider>());
            }

            return tagDex.MessagesForTags(query.Facets);
        }

    }
}
