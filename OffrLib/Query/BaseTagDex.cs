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
        protected Dictionary<string, List<IMessage>> _index;
        protected Dictionary<string, List<IMessage>> _doubleTagIndex;

        protected BaseTagDex()
        {
            _seenTags = new List<ITag>();
            _index = new Dictionary<string, List<IMessage>>();
            _doubleTagIndex = new Dictionary<string, List<IMessage>>();
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

        public List<IMessage> MessagesForTags(List<ITag> tags)
        {
            Update();
        
            List<ITag> intersectTags = new List<ITag>();
            foreach (ITag tag in tags)
            {
                intersectTags.Add(tag);
            }

            IEnumerable<IMessage> candidates;
            //if (intersectTags.Count > 1)
            //{
            //    string doubleKey = GetDoubleKey(intersectTags[0], intersectTags[1]);
            //    candidates = _doubleTagIndex.ContainsKey(doubleKey) ? _doubleTagIndex[doubleKey] : new List<IMessage>();
            //}
            //else 
            if (intersectTags.Count > 0)
            {
                candidates = _index.ContainsKey(intersectTags[0].match_tag) ? _index[intersectTags[0].match_tag] : new List<IMessage>();
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
            //FIXME bad place to put this code?
            // give results in reverse order 
            results.Reverse();
            return results;
        }

        public void Process(IEnumerable<IMessage> messages)
        {
            foreach (IMessage message in messages)
            {
                foreach (ITag tag1 in message.Tags)
                {
                    lock (_index)
                    {
                        if (!_index.ContainsKey(tag1.match_tag))
                        {
                            //FIXME this could be a bunch smarter
                            _index[tag1.match_tag] = new List<IMessage>();
                            _seenTags.Add(tag1);
                        }
                        _index[tag1.match_tag].Add(message);
                    }

                    lock (_doubleTagIndex)
                    {
                        foreach (ITag tag2 in message.Tags)
                        {
                            if (tag2.Equals(tag1)) continue;
                            string doubleKey = GetDoubleKey(tag1, tag2);

                            if (!_doubleTagIndex.ContainsKey(doubleKey))
                            {
                                _doubleTagIndex[doubleKey] = new List<IMessage>();
                            }
                            _doubleTagIndex[doubleKey].Add(message);
                        }
                    }
                }
            }
        }

        public TagWithCount GetTagCountForTag(ITag tag)
        {
            if (_index.ContainsKey(tag.match_tag))
            {
                return new TagWithCount() { count = _index[tag.match_tag].Count(), tag = tag };
            }
            else
            {
                return new TagWithCount() { count=0, tag = tag };
            }
        }

        private static string GetDoubleKey(ITag tag1, ITag tag2)
        {
            if (tag1.match_tag.CompareTo(tag2.match_tag) < 1)
            {
                return tag1.match_tag + tag2.match_tag;
            }
            else
            {
                return tag2.match_tag + tag1.match_tag;
            }
        }

       
    }
   
}
