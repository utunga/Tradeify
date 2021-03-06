﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Offr.Json.Converter
{
    public interface ICanJsonObject
    {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
        void ReadJson(JObject jObject, JsonSerializer serializer);
    }
}
