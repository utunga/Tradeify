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

        
    }
}
