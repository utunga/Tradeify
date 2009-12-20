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
            //FIXME1 really, we need to create the right TYPE of RawMessage here
            //       then we can base RawMessage abstract, and mroe to the point
            //       RawMessages won't lose their type..
            return new RawMessage();;
        }
    }
}
