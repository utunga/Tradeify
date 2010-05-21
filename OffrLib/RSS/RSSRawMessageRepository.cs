using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Location;
using Offr.Repository;

namespace Offr.RSS
{
    public interface IRSSRawMessageRepository
    {
        RSSRawMessage Get(string id);
        bool Has(string id);
        void Save(RSSRawMessage instance);
        void Remove(RSSRawMessage instance);
        void Remove(string id);
    }

    public class RSSRawMessageRepository : BaseRepository<RSSRawMessage>, IRSSRawMessageRepository
    {
        public bool Has(string id)
        {
            return (_list.ContainsKey(id));
        }
    }
}
