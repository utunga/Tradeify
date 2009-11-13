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

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            IRawMessage value = Create(objectType);
            if (value == null)
                throw new JsonSerializationException("No object created.");

            value = null;
            //serializer.Populate(reader, value);
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            serializer.Serialize(writer, value);
        }



    }
}
