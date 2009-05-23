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
        private readonly ITagProvider _tagProvider;

        private Dictionary<ITag, List<IMessage>> _index;
        private Dictionary<string, List<IMessage>> _doubleTagIndex;

        public TagDex(IMessageProvider messageProvider, ITagProvider tagProvider)
        {
            _messageProvider = messageProvider;
            _tagProvider = tagProvider;
            Invalidate(); // call invalidate to initialize data with basic messages
        }

        public List<IMessage> MessagesForTags(List<string> facets)
        {
            if (_messageProvider != null)
            {
                _messageProvider.Update();
                // may end up calling back into the 'process' method here
            }

            List<ITag> intersectionTags = new List<ITag>();
            foreach (string facet in facets)
            {
                intersectionTags.Add(_tagProvider.FromString(facet));
            }

            ////FIXME this could be a bunch smarter
            ////first sort by tag count on the intersectionTags
            //intersectionTags.Sort(TagCountSorter);
            //foreach(List<string) tags in unionTags.Values) {
            //    tags.Sort(TagCountSorter);
            //}

            IList<IMessage> candidates;
            if (intersectionTags.Count > 1)
            {
                string doubleKey = GetDoubleKey(intersectionTags[0], intersectionTags[1]);
                candidates = _doubleTagIndex.ContainsKey(doubleKey) ? _doubleTagIndex[doubleKey] : new List<IMessage>();
            }
            else if (intersectionTags.Count > 0)
            {
                candidates = _index.ContainsKey(intersectionTags[0]) ? _index[intersectionTags[0]] : new List<IMessage>();
            }
            else
            {
                candidates = _messageProvider.AllMessages;
            }

            List<IMessage> results = new List<IMessage>();
            foreach (IMessage message in candidates)
            {
                bool include = true;
                foreach (ITag tag in intersectionTags)
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
            var results = new SortedList<string, TagWithCount>();

            foreach (ITag tag in _index.Keys)
            {
                int count = _index[tag].Count;
                results.Add(tag.match_tag, new TagWithCount() { tag = tag, count = count });
                total += count;
            }
            return new TagCounts() { Tags = results, Total = total };
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
            Process(_messageProvider.AllMessages);
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
