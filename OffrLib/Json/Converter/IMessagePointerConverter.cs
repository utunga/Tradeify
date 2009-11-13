using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Message;
using Offr.Twitter;

namespace Offr.Json.Converter
{
    public class IMessagePointerConverter : CustomCreationConverter<IMessagePointer>
    {

        public override IMessagePointer Create(Type objectType)
        {
            return new TwitterMessagePointer();

        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            serializer.Serialize(writer, value);
        }
    }
}
