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

        public TagDexQueryExecutor(IMessageProvider messageProvider)
        {
            _globalTagIndex = new MessageReceivingTagDex(messageProvider);
        }

        public IEnumerable<IMessage> GetMessagesForTags(IEnumerable<ITag> tags)
        {
            return _globalTagIndex.MessagesForTags(tags);
        }

        public IEnumerable<IMessage> GetMessagesForTagsCreatedByUser(IEnumerable<ITag> tags, IUserPointer userPointer)
        {
            return _globalTagIndex.MessagesForTagsAndUser(tags, userPointer);
        }

        public IEnumerable<IMessage> GetMessagesCreatedByUser(IUserPointer userPointer)
        {
            return _globalTagIndex.MessagesForUser(userPointer);
        }
        
        //public IEnumerable<IMessage> GetMessagesForKeywordAndTags(string keyword, IEnumerable<ITag> tags)
        //{
        //    // theoretically if we were caching all these objects this use of a 
        //    // MessageReceivingTagDex would mean that we get up dates from the feed
        //    // when new messages match the query, but.. since we don't cache it anyway, that
        //    // is kind of pointless
        //    //MessageProviderForKeywords 
        //    MessageProviderForKeywords keywordMessageProvider = Global.Kernel.Get<MessageProviderForKeywords>(With.Parameters.ConstructorArgument("keywords", tags));
        //    MessageReceivingTagDex tagDex = new MessageReceivingTagDex(keywordMessageProvider);
        //    return tagDex.MessagesForTags(tags);
        //}

        //public TagCounts GetTagCountsForKeywordAndTags(string keyword, IEnumerable<ITag> tags)
        //{
        //    MessageProviderForKeywords keywordMessageProvider = Global.Kernel.Get<MessageProviderForKeywords>(With.Parameters.ConstructorArgument("keywords", tags));
        //    StaticTagDex tagDex = new StaticTagDex(keywordMessageProvider.AllMessages);
        //    return tagDex.GetTagCounts();
        //}

        public TagCounts GetTagCountsForTags(IEnumerable<ITag> tags)
        {
     
            var tagCounts = new List<TagWithCount>();
            foreach (ITag tag in tags)
            {
                tagCounts.Add(_globalTagIndex.GetTagCountForTag(tag));
            }

            //tagCounts.Reverse();
            return new TagCounts() { Tags = tagCounts, Total = -1 };
        }

        public TagCounts GetTagCounts()
        {
            return _globalTagIndex.GetTagCounts();
        }
    }
}
