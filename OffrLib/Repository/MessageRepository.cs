using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.Repository;
using Offr.Json.Converter;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Repository
{
    public class MessageRepository : BaseRepository<IMessage>, IMessageRepository, IPersistedRepository
    {
        public MessageRepository()
        {
        }

        public IEnumerable<IMessage> AllMessages()
        {
            return base.GetAll();
        }
    }
}