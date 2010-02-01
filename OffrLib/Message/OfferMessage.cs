using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public class OfferMessage : BaseMarketMessage, IOfferMessage
    {
        public override MessageType MessageType
        {
            get { return MessageType.offer; }
        }

        public OfferMessage() : base()
        {
            base.AddTag(new Tag(TagType.msg_type, MessageType.offer.ToString()));
        }

    }
}
