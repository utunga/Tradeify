using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;

namespace Offr.Text
{
    public class TagProvider : ITagProvider
    {
        // one day could implement this as a caching lookup, with counts attached etc..
        //but for now just parse the string to get the tag
        //should be a dictionary with tag values
        private Dictionary<String, ITag> _knownTags;
        
        public TagProvider()
        {
            _knownTags = new Dictionary<string, ITag>();
            AddKnownTags();
            }

        private void AddKnownTags()
        {
            foreach (MessageType msgType in Enum.GetValues(typeof(MessageType)))
            {
                string tagText = msgType.ToString();
                _knownTags.Add(tagText, FromTypeAndText(TagType.msg_type,tagText));
            }
            _knownTags.Add("ooooby", FromTypeAndText(TagType.group,"ooooby"));
            _knownTags.Add("freecycle", FromTypeAndText(TagType.group, "freecycle"));
            _knownTags.Add("cash only", FromTypeAndText(TagType.type, "cash only"));
            _knownTags.Add("cash", FromTypeAndText(TagType.type, "cash"));
            _knownTags.Add("nzd", FromTypeAndText(TagType.type, "nzd"));
            _knownTags.Add("barter", FromTypeAndText(TagType.type, "barter"));
            _knownTags.Add("free", FromTypeAndText(TagType.type, "free"));
        }

        private ITag FromTypeAndText(TagType tagType, string tagText)
        {
            return new Tag(tagType, tagText);
        }

         public ITag GetTag(string tagString)
        {
            string tagStringLowerCase = tagString.ToLowerInvariant();
            ITag tag;
            if (_knownTags.TryGetValue(tagStringLowerCase, out tag))
            {
                return tag;  
            }
            else
            {
                ITag newTag = FromTypeAndText(TagType.tag, tagStringLowerCase);
                _knownTags.Add(tagStringLowerCase, newTag);
                return newTag;
            }

        }
    }
}
