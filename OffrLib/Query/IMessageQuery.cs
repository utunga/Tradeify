using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Location;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    public interface IMessageQuery
    {
        string Keywords { get; }
        //ILocation Location { get; }
        List<ITag> Facets { get; }
        //IEnumerable<IMessage> ExecuteQuery(IQueryable<IMessage> messages);
    }
}
