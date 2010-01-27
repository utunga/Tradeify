﻿using System;
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
    public class MessageRepository : BaseRepository<IMessage>, IMessageRepository, IMessageQueryExecutor, IPersistedRepository, IEnumerable<IMessage>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly TagDex _globalTagIndex;
        private readonly IUserPointerRepository _ignoredUsers;

        public MessageRepository(IUserPointerRepository ignoredUsers)
        {
            _ignoredUsers = ignoredUsers;
            _globalTagIndex = new TagDex(this);
        }

        /// <summary>
        /// Constructor that doesn't allow for blocking of invalid messages
        /// </summary>
        [Obsolete("Only used in testing, we should remove this call")]
        public MessageRepository() : this(new IgnoredUserRepository())
        {
        }

        public int MessageCount
        {
            get
            {
                return base.Count;
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

        public MessagesWithTagCounts GetMessagesWithTagCounts(IEnumerable<ITag> tags)
        {
            return new MessagesWithTagCounts(QueryMessagesImpl(tags, null));
        }

        public TagCounts GetAllTagCounts()
        {
            //NOTE2JOAV
            // this is the method that may be worth caching
            MessagesWithTagCounts allMessages = GetMessagesWithTagCounts(new Tag[] {});
            return allMessages.TagCounts;
        }

        public IEnumerable<IMessage> GetMessagesForTags(IEnumerable<ITag> tags)
        {
            return QueryMessagesImpl(tags, null);
        }

        public IEnumerable<IMessage> GetMessagesCreatedByUser(IUserPointer user)
        {
            return QueryMessagesImpl(new ITag[] { }, user);
        }

        public IEnumerable<IMessage> GetMessagesForTagsCreatedByUser(IEnumerable<ITag> tags, IUserPointer user)
        {
            return QueryMessagesImpl(tags, user);
        }

        private List<IMessage> QueryMessagesImpl(IEnumerable<ITag> tags, IUserPointer user)
        {
            List<IMessage> results = new List<IMessage>();
            foreach (IMessage message in _globalTagIndex.QueryMessages(tags, user))
            {
                
                //check whether the offer is expired
                if (message.IsExpired()) continue;
                //check if user is in ignored user list
                if (_ignoredUsers.Get(message.CreatedBy.ID) == null)
                {
                    results.Add(message);
                }
            } 

            //sort results by timestamp descending
            results.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return results;
        }

        //public TagCounts GetTagCounts()
        //{
        //    return _globalTagIndex.GetTagCounts();
        //}

        //public TagCounts GetTagCountsForTags(IEnumerable<ITag> tags)
        //{
        //    var tagCounts = new List<TagWithCount>();
        //    foreach (ITag tag in tags)
        //    {
        //        tagCounts.Add(_globalTagIndex.GetTagCountForTag(tag));
        //    }

        //    //tagCounts.Reverse();
        //    return new TagCounts() { Tags = tagCounts, Total = -1 };
        //}

        public IEnumerator<IMessage> GetEnumerator()
        {
            return base.GetAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /*
          *Fruit
Vegetables
Eggs
Honey
Nuts & Beans
Grains
--
Food Growing Books
Food Growing Consultancy
Food Transport
Food Trees
Gardening and Yard Work
Glass/Shade Houses
Incidentals; Piping, Net, etc.
Prepared Food
Recipe Books
Seedlings
Seeds
Soils & Fertilisers
Teaching
Tools & Equipment
Workshops
Miscellaneous  
          */
        
        public void InitializeFromFile()
        {
            
            //addDummyMessages();
            //msg.AddThumbnail(GetImageUrl(sourceText));
            base.InitializeFromFile();
        }
         
        private void addDummyMessages()
        {
            var dummy = new OpenSocialUserPointer("ooooby", "Dummy",
                                               "http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg",
                                               "");

            saveDummyMessage(dummy, "ooooby", "Vegetables");
            saveDummyMessage(dummy, "ooooby", "Fruit");
            saveDummyMessage(dummy, "ooooby", "Eggs");
            saveDummyMessage(dummy, "ooooby", "Honey");
            saveDummyMessage(dummy, "ooooby", "Nuts & Beans");
            saveDummyMessage(dummy, "ooooby", "Grains");
            saveDummyMessage(dummy, "ooooby", "Food Growing Books");

        }
        private void saveDummyMessage(IUserPointer dummy, string group, string term)
        {
            for (int i = 0; i < 10; i++)
            {
                OfferMessage msg = new OfferMessage();
                msg.CreatedBy = dummy;
                ////msg.Source = source; //Remove this
                msg.Timestamp = DateTime.MinValue;
                msg.MessagePointer = new OpenSocialMessagePointer("ooooby");
                msg.RawText = "";
                msg.OfferText = "";
                msg.MoreInfoURL = "";
                msg.AddTag(new Tag(TagType.tag, group));
                msg.AddTag(new Tag(TagType.tag, term));
                this.Save(msg);
            }
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