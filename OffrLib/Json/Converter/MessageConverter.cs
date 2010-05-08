using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Message;

namespace Offr.Json.Converter
{
    public class MessageConverter : CanJsonConvertor<IMessage>
    {
        public override IMessage Create(JsonReader reader, JsonSerializer serializer)
        {
            string type = JSON.ReadProperty<string>(serializer, reader, "message_type");
            log.Info("Trying to create object of type: " + type);
            if (type.Equals(MessageType.offer.ToString()))
                return new OfferMessage();
            else if (type.Equals(MessageType.wanted.ToString()))
                return new WantedMessage();
           throw new JsonReaderException("Failed to recognize Message of type:" + type);
        }
    }
}
