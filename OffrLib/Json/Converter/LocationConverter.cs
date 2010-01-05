using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Location;

namespace Offr.Json.Converter
{
    public class LocationConverter : CanJsonConvertor<ILocation>
    {

        public override ILocation Create(JsonReader reader, JsonSerializer serializer)
        {
            return new Location.Location();
        }
    }
}
