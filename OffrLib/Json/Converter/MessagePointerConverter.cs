using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Message;
using Offr.Twitter;

namespace Offr.Json.Converter
{
    public class MessagePointerConverter : CanJsonConvertor<IMessagePointer>
    {

        public override IMessagePointer Create(JsonReader reader)
        {
            return new TwitterMessagePointer();
        }
    }
}
