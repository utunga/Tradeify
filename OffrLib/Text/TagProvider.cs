using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public class TagProvider : ITagProvider
    {
        // one day could implement this as a caching lookup, with counts attached etc..
        //but for now just parse the string to get the tag

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
    }
}
