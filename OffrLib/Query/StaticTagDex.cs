using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Common;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    public class StaticTagDex : BaseTagDex
    {
        private readonly IEnumerable<IMessage> _messages;
    
        public StaticTagDex(IEnumerable<IMessage> messages)
        {
            _messages = messages;
            Process(_messages); // call invalidate to initialize data with explictly provided messages, won't update past this point
        }

        public TagCounts GetTagCounts()
        {
            var tags = new List<TagWithCount>();
            foreach (ITag tag in _seenTags)
            {
                int count = _index[tag.match_tag].Count;
                tags.Add(new TagWithCount() { count = count, tag = tag });
            }
            return new TagCounts() { Tags = tags, Total = _messages.Count() };
        }

        #region Overrides of BaseTagDex

        protected override IEnumerable<IMessage> AllMessages()
        {
            return _messages;
        }

        protected override void Update()
        {
            //ignore
        }

        #endregion
    }
}
