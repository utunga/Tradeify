using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Location;

namespace Offr.Json.Converter
{
    public class ILocationConverter : CustomCreationConverter<ILocation>
    {

        public override ILocation Create(Type objectType)
        {
            return new Location.Location();

        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            serializer.Serialize(writer, value);
        }
    }
}
