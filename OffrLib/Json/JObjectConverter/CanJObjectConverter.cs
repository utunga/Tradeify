using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Offr.Json.Converter;

namespace Offr.Json.JObjectConverter
{
    public class CanJObjectConverter<T> : JsonConverter where T:ICanJsonObject
    {
        protected static readonly Logger log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// CanJObjectConverter can convert any object implementing CanJson
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Creates an object which will then be populated by the serializer.
        /// </summary>
        /// <returns></returns>
        public virtual T Create(JObject reader, JsonSerializer serializer)
        {
            return default(T);
        }

        /// <summary>
        /// Writes the JSON representation of the object by calling the WriteJson method on the target
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is ICanJsonObject))
            {
                throw new SerializationException("Cannot convert object " + value + "expected something that implements ICanJsonObject");
            }
            writer.WriteStartObject();
            ((ICanJsonObject)value).WriteJson(writer, serializer);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Reads the JSON representation by creating a new x and calling the ReadJson method on the target
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jObject = JObject.Load(reader);
            ICanJsonObject value = Create(jObject, serializer);
            if (value == null)
                throw new JsonSerializationException("No object created.");

            value.ReadJson(jObject, serializer);
            //JSON.ReadAndAssert(reader);
            //it may be that you prefer an JObject here in which case
            // 
            return value;
        }


    }
}