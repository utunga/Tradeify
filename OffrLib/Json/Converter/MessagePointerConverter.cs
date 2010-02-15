using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Message;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Json.Converter
{
    public class MessagePointerConverter : CanJsonConvertor<IMessagePointer>
    {

        public override IMessagePointer Create(JsonReader reader, JsonSerializer serializer)
        {
            string type = JSON.ReadProperty<string>(serializer, reader, "provider_name_space");
            if (type.Equals("twitter"))
                return new TwitterMessagePointer();
            else return new OpenSocialMessagePointer(type);
            
        }
    }
}
