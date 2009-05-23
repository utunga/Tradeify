using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Offr.Location;

namespace Offr.Query
{
    public class MessageQuery : IMessageQuery
    {
        public string Keywords { get; set; }
        //public ILocation Location { get; set; }
        public List<string> Facets { get; set; }

        public MessageQuery()
        {
            Facets = new List<string>();
        }

        public override string ToString()
        {
            return "query<keyword:" + Keywords + " facets:" + string.Join(",", Facets.ToArray()) + ">";
        }

        //FIXME really we want to do this with MVC routing, but for now..
        public static MessageQuery FromNameValCollection(NameValueCollection nameVals)
        {
            MessageQuery query = new MessageQuery();
            if (nameVals["q"]!=null)
            {
                query.Keywords = nameVals["q"];
            }
            if (nameVals.GetValues("f")!=null) {
                foreach (string facet in nameVals.GetValues("f"))
                {
                    query.Facets.Add(facet);
                }
            }
            return query;
        }
    }
}
