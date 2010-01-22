using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Json.Converter;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Repository
{
    public class MessageRepository : BaseRepository<IMessage>, IMessageRepository, IPersistedRepository, IEnumerable<IMessage>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly TagDex _globalTagIndex;

        public MessageRepository(HashSet<IUserPointer> ignoredUsers)
        {
            _globalTagIndex = new TagDex(this,ignoredUsers);
        }

        public MessageRepository()
        {
            _globalTagIndex = new TagDex(this);    
        }

        public int MessageCount
        {
             get
             {
                 return base._list.Count;   
             }   
        }

        public IEnumerable<IMessage> AllMessages()
        {
            return base.GetAll();
        }

        public override void Save(IMessage message)
        {
            base.Save(message);
            _log.Info("Save:" + message);
            _globalTagIndex.Process(message);
        }

        public IEnumerable<IMessage> GetMessagesForTags(IEnumerable<ITag> tags, bool includeIgnoredUsers)
        {
            return _globalTagIndex.MessagesForTags(tags,includeIgnoredUsers);
        }

        public IEnumerable<IMessage> GetMessagesForTagsCreatedByUser(IEnumerable<ITag> tags, IUserPointer userPointer)
        {
            //not sure if 'true' is correct
            return _globalTagIndex.MessagesForTagsAndUser(tags, userPointer,true);
        }

        public IEnumerable<IMessage> GetMessagesCreatedByUser(IUserPointer userPointer)
        {
            return _globalTagIndex.MessagesForUser(userPointer);
        }

        public TagCounts GetTagCounts()
        {
            return _globalTagIndex.GetTagCounts();
        }

        public IEnumerator<IMessage> GetEnumerator()
        {
            return base.GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
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

    }
}