using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Offr.Common;
using Offr.Message;
using Offr.Repository;
using Offr.Text;

namespace Offr.Query
{

    public class TagDex
    {
        private readonly List<ITag> _seenTags;
        private readonly SortedList<string, List<IMessage>> _index;
        private IEnumerable<IMessage> _allMessageSource;
        
        public TagDex(IEnumerable<IMessage> allMessageSource)
        {
            _seenTags = new List<ITag>();
            _index = new SortedList<string, List<IMessage>>();
            _allMessageSource = allMessageSource;
            Process(allMessageSource);
        }
        public void Reprocess(IEnumerable<IMessage> allMessageSource)
        {
             _seenTags.Clear();
            _index.Clear();
            _allMessageSource = allMessageSource;
            Process(allMessageSource);
        }
        /// <summary>
        /// From 
        /// </summary>
        /// <param name="messages"></param>
        public void Process(IEnumerable<IMessage> messages)
        {
            foreach (IMessage message in messages)
            {
                Process(message);
            }
        }

        public void Process(IMessage message)
        {
            lock (_index)
            {
                // store an index of messages by tag
                foreach (ITag tag1 in message.Tags)
                {
                    if (!_index.ContainsKey(tag1.MatchTag))
                    {
                        _index[tag1.MatchTag] = new List<IMessage>();
                        _seenTags.Add(tag1);
                    }
                    _index[tag1.MatchTag].Add(message);
                }

                //also index by user
                if (message.CreatedBy != null)
                {
                    string matchTag = message.CreatedBy.MatchTag;
                    if (!_index.ContainsKey(matchTag))
                    {
                        _index[matchTag] = new List<IMessage>();
                    }
                    _index[matchTag].Add(message);
                }
            }
        }
        public IEnumerable<IMessage> QueryMessages(IEnumerable<ITag> tags, IUserPointer user)
        {
            List<string> matchTags = tags.Select(tag => tag.MatchTag).ToList();
            if (user != null)
            {
                matchTags.Add(user.MatchTag);
            }

            IEnumerable<IMessage> candidates;
            if (matchTags.Count > 0)
            {
                candidates = _index.ContainsKey(matchTags[0]) ? _index[matchTags[0]] : new List<IMessage>();
            }
            else
            {
                candidates = _allMessageSource;
            }

            foreach (IMessage message in candidates)
            {
                bool include = true;
                foreach (string matchTag in matchTags)
                {
                    if (!message.MatchesMatchTag(matchTag))
                    {
                        include = false;
                        break;
                    }
                }
                if (include)
                {
                    yield return message;
                }
            }
        }
 
        public TagWithCount GetTagCountForTag(ITag tag)
        {
            return _index.ContainsKey(tag.MatchTag) ? 
                new TagWithCount() { count = _index[tag.MatchTag].Count(), tag = tag } :
                new TagWithCount() { count=0, tag = tag };
        }

        //get the 'total' for a TagCounts object based on just adding up the number of messages with each tag
        public TagCounts GetTagCounts()
        {
            List<TagWithCount> tagCounts = new List<TagWithCount>();
            HashSet<IMessage> messageSet = new HashSet<IMessage>();
            foreach (ITag tag in _seenTags)
            {
                List<IMessage> messagesOfTag = _index[tag.MatchTag];
                int count = messagesOfTag.Count;
                foreach (IMessage message in messagesOfTag)
                {
                    messageSet.Add(message);
                }
                tagCounts.Add(new TagWithCount() {count = count, tag = tag});
            }
            return new TagCounts() {Tags = tagCounts, Total = messageSet.Count};
        }
    }
}
