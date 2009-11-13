using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Message;

namespace Offr.Json.Converter
{
    public class IMessageConverter : CustomCreationConverter<IMessage>
    {

        public override IMessage Create(Type objectType)
        {
            return new OfferMessage();

        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            serializer.Serialize(writer, value);
        }

    }
}
