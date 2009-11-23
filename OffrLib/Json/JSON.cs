using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json.Converter;

namespace Offr.Json
{
    public class JSON
    {
        static JsonSerializerSettings SerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            }
        }
        
        private static JsonSerializer Serializer
        {
            get
            {
                JsonSerializer jsonSerializer = JsonSerializer.Create(SerializerSettings);

                //register all our favorite convertors
                jsonSerializer.Converters.Add(new IsoDateTimeConverter());
                jsonSerializer.Converters.Add(new RawMessageConverter());
                jsonSerializer.Converters.Add(new LocationConverter());
                jsonSerializer.Converters.Add(new MessageConverter());
                jsonSerializer.Converters.Add(new MessagePointerConverter());
                jsonSerializer.Converters.Add(new TagListConverter());
                jsonSerializer.Converters.Add(new TagConverter());
                jsonSerializer.Converters.Add(new UserPointerConverter());
                return jsonSerializer;
            }
        }
  
        public static string Serialize(Object toSerialize)
        {
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                Serializer.Serialize(jsonWriter, toSerialize);
            }
            return sw.ToString();
        }

        //public static String Serialize(Object toSerialize, params JsonConverter[] converters)
        //{
        //    return JsonConvert.SerializeObject(toSerialize,converters);
        //}

        public static T Deserialize<T>(string toDeserialize)
        {
            StringReader sr = new StringReader(toDeserialize);

            object deserializedValue;

            using (JsonReader jsonReader = new JsonTextReader(sr))
            {
                deserializedValue = Serializer.Deserialize(jsonReader, typeof(T));

                if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
                    throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
            }

            return (T)deserializedValue;
        }

        //public static T Deserialize<T>(string value, params JsonConverter[] converters)
        //{
        //    return JsonConvert.DeserializeObject<T>(value, converters);
        //}

        public static T ReadProperty<T>(JsonSerializer serializer, JsonReader reader, string propertyName)// where  T : Nullable
        {
            ReadAndAssertProperty(reader, propertyName);
            ReadAndAssert(reader);
            //if (reader.TokenType == JsonToken.Null) return null as T;
            return (T)serializer.Deserialize(reader, typeof(T));
        }

        public static void WriteProperty(JsonSerializer serializer, JsonWriter writer, string property, object value)
        {
            writer.WritePropertyName(property);
            serializer.Serialize(writer, value);
        }
        
        public static void ReadAndAssertProperty(JsonReader reader, string propertyName)
        {
            ReadAndAssert(reader);

            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != propertyName)
                throw new JsonSerializationException(string.Format("Expected JSON property '{0}'", propertyName));
        }

        public static void ReadAndAssertStringValue(JsonReader reader, string propertyName)
        {
            ReadAndAssert(reader);

            if (reader.TokenType != JsonToken.String || reader.Value.ToString() != propertyName)
                throw new JsonSerializationException(string.Format("Expected JSON property '{0}'.", propertyName));
        }

        public static void ReadAndAssert(JsonReader reader)
        {
            if (!reader.Read())
                throw new JsonSerializationException("Unexpected end.");
        }

       

        static JSON()
        {
            
        }
    }


}
