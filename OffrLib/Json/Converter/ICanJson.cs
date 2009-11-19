using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Offr.Json.Converter
{
    public interface ICanJson
    {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
        void ReadJson(JsonReader reader, JsonSerializer serializer);
    }
}
