using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Offr.Location;
using Offr.Text;

namespace Offr.Query
{
    public class MessageQuery : IMessageQuery
    {
        public string Keywords { get; set; }
        //public ILocation Location { get; set; }
        public List<ITag> Facets { get; set; }

        public MessageQuery()
        {
            Facets = new List<ITag>();
        }

        public override string ToString()
        {
            string toString = "query<keyword:" + Keywords;
            toString += " facets:";
            foreach (ITag tag in Facets)
            {
                 toString += tag.match_tag;
            }
            toString += ">";
            return toString;
        }



        public static MessageQuery MessageQueryFromNameValCollection(ITagProvider tagProvider, NameValueCollection nameVals)
        {
            MessageQuery query = new MessageQuery();
            if (nameVals["q"]!=null)
            {
                query.Keywords = nameVals["q"];
            }
            query.Facets = ParseTagsFromNameVals(tagProvider, nameVals);
            return query;
        }

        public static List<ITag> ParseTagsFromNameVals(ITagProvider tagProvider, NameValueCollection nameVals)
        {
           
            List<ITag> tags = new List<ITag>();
            foreach (TagType tagType in Enum.GetValues(typeof(TagType)))
            {
                if (nameVals.GetValues(tagType.ToString()) != null)
                {
                    foreach (string tagText in nameVals.GetValues(tagType.ToString()))
                    {
                        ITag tag = tagProvider.FromTypeAndText(tagType, tagText);
                        tags.Add(tag);
                    }
                }
            }
            return tags;
        }
    }
}
