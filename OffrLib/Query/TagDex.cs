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

    public class TagDex : IMemCache, IMessageReceiver
    {
        private readonly IMessageProvider _messageProvider;
        private readonly IEnumerable<IMessage> _explicitMessages;

        private Dictionary<ITag, List<IMessage>> _index;
        private Dictionary<string, List<IMessage>> _doubleTagIndex;

        public TagDex(IMessageProvider messageProvider)
        {
            _messageProvider = messageProvider;
            Invalidate(); // call invalidate to initialize data with messageProvider.AllMessages() // *will receive updates from then on
        }

        public TagDex(IEnumerable<IMessage> explicitMessages)
        {
            _explicitMessages = explicitMessages;
            Invalidate(); // call invalidate to initialize data with explictly provided messages, won't update past this point
        }

        public List<IMessage> MessagesForTags(List<ITag> tags)
        {
            if (_messageProvider != null)
            {
                _messageProvider.Update();
                // may end up calling back into the 'process' method here
            }

            List<ITag> intersectTags = new List<ITag>();
            foreach (ITag tag in tags)
            {
                if ((tag.type == TagType.tag)
                    || (tag.type == TagType.group))
                {
                    intersectTags.Add(tag);
                }
            }

            ////FIXME this could be a bunch smarter
            ////first sort by tag count on the intersectionTags
            //intersectionTags.Sort(TagCountSorter);
            //foreach(List<string) tags in unionTags.Values) {
            //    tags.Sort(TagCountSorter);
            //}

            IEnumerable<IMessage> candidates;
            if (intersectTags.Count > 1)
            {
                string doubleKey = GetDoubleKey(intersectTags[0], intersectTags[1]);
                candidates = _doubleTagIndex.ContainsKey(doubleKey) ? _doubleTagIndex[doubleKey] : new List<IMessage>();
            }
            else if (intersectTags.Count > 0)
            {
                candidates = _index.ContainsKey(intersectTags[0]) ? _index[intersectTags[0]] : new List<IMessage>();
            }
            else
            {
                if (_messageProvider != null)
                {
                    // may end up calling back into the 'process new messages' method here
                    candidates = _messageProvider.AllMessages;
                }
                else if (_explicitMessages != null)
                {
                    candidates = _explicitMessages;
                }
                else
                {
                    candidates = new List<IMessage>(); // no candidates
                }
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

                // foreach union
                // foreach tag that is left in contention

                if (include)
                {
                    results.Add(message);
                }

            }
            return results;
        }

        public TagCounts GetTagCounts()
        {
            int total = 0;
            var results = new List<TagWithCount>();

            foreach (ITag tag in _index.Keys)
            {
                int count = _index[tag].Count;
                results.Add(new TagWithCount() { count = count, tag = tag });
                total += count;
            }
            return new TagCounts() {Tags = results, Total = total };
        }

        public void Process(IEnumerable<IMessage> messages)
        {
            foreach (IMessage message in messages)
            {
                foreach (ITag tag1 in message.Tags)
                {
                    lock (_index)
                    {
                        if (!_index.ContainsKey(tag1))
                        {
                            //FIXME this could be a bunch smarter
                            _index[tag1] = new List<IMessage>();
                        }
                        _index[tag1].Insert(0, message);
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
                            _doubleTagIndex[doubleKey].Insert(0, message);
                        }
                    }
                }
            }
        }

        #region Implementation of IMemCache

        public void Invalidate()
        {
            _index = new Dictionary<ITag, List<IMessage>>();
            _doubleTagIndex = new Dictionary<string, List<IMessage>>();

            //FIXME: definitely should be split into two sepeate classes
            if (_messageProvider != null)
            {
                Process(_messageProvider.AllMessages);
            }
            else if (_explicitMessages != null)
            {
                Process(_explicitMessages);
            }
        }

        #endregion

        #region Implementation of IMessageReceiver

        public void Notify(IEnumerable<IMessage> updatedMessages)
        {
            Process(updatedMessages);
        }

        #endregion

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
