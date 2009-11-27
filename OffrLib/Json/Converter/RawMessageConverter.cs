using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Text;

namespace Offr.Json.Converter
{
    public class RawMessageConverter : CanJsonConvertor<IRawMessage>
    {
        public override IRawMessage Create(JsonReader reader)
        {
            return new RawMessage();;
        }
    }
}
