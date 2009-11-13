using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Text;

namespace Offr.Json.Converter
{
    public class RawMessageConverter : CustomCreationConverter<IRawMessage>
    {

        public override IRawMessage Create(Type objectType)
        {
            return new RawMessage();

        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            serializer.Serialize(writer, value);
        }
    }
}
