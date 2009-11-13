using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Offr.Json
{
    public class JSONConverter
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

        public static String Serialize(Object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }

        public static T Deserialize<T>(String toDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(toDeserialize,SerializerSettings);
        }

        public static T Deserialize<T>(string value, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(value, converters);
        }
    }
}
