using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Offr.Common;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{

    public abstract class BaseTagDex
    {

        protected List<ITag> _seenTags;
        protected SortedList<string, List<IMessage>> _index;

        protected BaseTagDex()
        {
            _seenTags = new List<ITag>();
            _index = new SortedList<string, List<IMessage>>();
        }

        /// <summary>
        /// When this is called, implementing classes should ensure their data is as up to date as it can get when this is called
        /// (static implementation will just ignore this call)
        /// </summary>
        protected abstract void Update();

        /// <summary>
        /// Implementing classes will occasionally need to return all messages
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IMessage> AllMessages();

        public List<IMessage> MessagesForTags(IEnumerable<ITag> tags)
        {
            //No longer update
            //Update();
        
            List<ITag> intersectTags = new List<ITag>();
            foreach (ITag tag in tags)
            {
                intersectTags.Add(tag);
            }

            IEnumerable<IMessage> candidates;
            if (intersectTags.Count > 0)
            {
                candidates = _index.ContainsKey(intersectTags[0].MatchTag) ? _index[intersectTags[0].MatchTag] : new List<IMessage>();
            }
            else
            {
                candidates = AllMessages();
            }

            List<IMessage> results = new List<IMessage>();
            foreach (IMessage message in candidates)
            {
                bool include = true;
                foreach (ITag tag in intersectTags)
                {
                    if (!message.HasTag(tag))
                    {
                        include = false;
                        break;
                    }
                }

                if (include)
                {
                    results.Add(message);
                }
            }
            //sort results by timestamp descending
            results.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return results;
        }
   
        /// <summary>
        /// From 
        /// </summary>
        /// <param name="messages"></param>
        public void Process(IEnumerable<IMessage> messages)
        {
            foreach (IMessage message in messages)
            {
                foreach (ITag tag1 in message.Tags)
                {
                    lock (_index)
                    {
                        if (!_index.ContainsKey(tag1.MatchTag))
                        {
                            //FIXME this could be a bunch smarter
                            _index[tag1.MatchTag] = new List<IMessage>();
                            _seenTags.Add(tag1);
                        }
                        _index[tag1.MatchTag].Add(message);
                    }
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
            return new TagCounts() { Tags = tagCounts, Total = messageSet.Count };
    }

    }
   
}
