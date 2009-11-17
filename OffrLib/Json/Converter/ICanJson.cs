using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Offr.Json.Converter
{
    /// <summary>
    /// Basic capability to write and read myself using Newtonsoft.JSON.
    /// Writing is done using JsonWriter in a fast streaming fashion.
    /// Reading is done using JObject "DOM style".
    /// </summary>
    public interface ICanJson
    {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
        void ReadJson(JsonReader reader, JsonSerializer serializer); //JObject jObject); maybe use JObject if its seems necessary
    }
}
