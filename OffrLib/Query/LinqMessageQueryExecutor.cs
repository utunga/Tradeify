using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    //public class LinqMessageQueryExecutor : IMessageQueryExecutor
    //{
    //    private readonly IMessageProvider _messageProvider;

    //    public LinqMessageQueryExecutor(IMessageProvider messageProvider) {
    //        _messageProvider = messageProvider;
    //    }

    //    public IEnumerable<IMessage> GetMessagesForQuery(IMessageQuery query)
    //    {
    //        throw new NotImplementedException("Changed stucture now this code doesn't work");
    //        //IQueryable<IMessage> messages = (query.Keywords != null) ? _messageProvider.MessagesForKeywords(query.Keywords) : _messageProvider.AllMessages;
          
    //        //FIXME: this is going to be REALLY SLOW on large datasets
    //        //        1. using LINQ to objects is a bit of a mistake for starters (sure its all in memory but indexing not great and googling around appears that hand coding the alg will usually be much faster)
    //        //        2. we should make sure to put the *least* used tag first etc etc..
    //        //return messages.Where(msg => HasAllTags(msg, query.Facets));
            
    //        //NB profile of this with only 6 messages in db = "ran 9261 queries in 31252ms - average:3ms or 11669871ticks "
    //    }

    //    public bool HasAllTags(IMessage msg, IEnumerable<string> matchTags)
    //    {
    //        foreach (string wantedTag in matchTags)
    //        {
    //            bool found = false;
    //            foreach (ITag tag in msg.Tags)
    //            {
    //                if (tag.MatchTag.Equals(wantedTag, StringComparison.InvariantCultureIgnoreCase))
    //                {
    //                    found = true;
    //                    break;
    //                }
    //            }
    //            if (found == false)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //}

}
