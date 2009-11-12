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
        private Dictionary<String, TagType> _knownTags;
        public TagProvider()
        {
            _knownTags = new Dictionary<string, TagType>();
            AddKnownTags();

            }


        private void AddKnownTags()
        {
            foreach (MessageType msgType in Enum.GetValues(typeof(MessageType)))
            {
                _knownTags.Add(msgType.ToString(),TagType.msg_type);
            }
            _knownTags.Add("ooooby", TagType.group);
            _knownTags.Add("freecycle", TagType.group);
            _knownTags.Add("cash only", TagType.type);
            _knownTags.Add("cash", TagType.type);
            _knownTags.Add("nzd", TagType.type);
            _knownTags.Add("barter", TagType.type);
            _knownTags.Add("free", TagType.type);
          /*  foreach (MessageType msgType in Enum.GetValues(typeof (MessageType)))
            {
                if (msgType.ToString().Equals(tagString))
                {
                    // for messages where we flag the type using a hash tag "eg #offr"
                    _knownTags.Add(tagString, TagType.msg_type);
                    return TagType.msg_type;
                }
            }

            switch (tagString.ToLowerInvariant())
            {
                case "ooooby":
                case "freecycle":
                    return TagType.group;

                case "cash only":
                case "cash":
                case "nzd":
                case "barter":
                case "free":
                    return TagType.type;

                default:
                    return TagType.tag;
            }*/
        }

        public ITag FromString(string match_tag)
        {
            if (!match_tag.Contains("/"))
            {
                match_tag = "tag/" + match_tag;
            }

            string[] parts = match_tag.Split('/');
            string tagTypeStr = parts[0];
            TagType tagType = (TagType) Enum.Parse(typeof (TagType), tagTypeStr, true);
            // if there were multiple 'types' one could also do a switch here, i suppose
            return new Tag(tagType, parts[1]);
        }

        public ITag FromTypeAndText(TagType tagType, string tagText)
        {
            return new Tag(tagType, tagText);
        }
        //NOTE2J please do the following
        // 1. move this code into the tag provider (change signature of _tagProvider.FromTypeAndText(type, tagString);
        //    to _tagProvider.GetTag(tagString); 
        // 2. rather than guessing the tag type based on this set list
        //    do the following:
        //      i. check to see if the tag string has already been assigned
        //     ii. if so return that tag, which will therefore have the specified type
        //    iii. if not, create a new tag of type 'tag' and add to the list of known tags
        //     iv. in the constructor for TagProvider, create a list of special per the switch (and the loop below)
        //      v. later, (or if you get time) we will serialize this list of special/defined tags from an init file
        //     vi. even later, we will replace the init file with a proper database back end (ie TagRepository)

        public TagType GetTag(String tagString)
        {
            //Note2Mt I have made all known tags lower case, I assume that you wont ever want to add different results 
            //for different case tags
            string tagStringLowerCase = tagString.ToLowerInvariant();
            TagType tag;
            if (_knownTags.TryGetValue(tagStringLowerCase, out tag))
            {
                return tag;  
            }
            else
            {
                _knownTags.Add(tagStringLowerCase, TagType.tag);
                return TagType.tag;
            }

        }
    }
}
