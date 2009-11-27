using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Message;

namespace Offr.Json.Converter
{
    public class MessageConverter : CanJsonConvertor<IMessage>
    {
        public override IMessage Create(JsonReader reader)
        {
            return new OfferMessage();
        }
    }
}
