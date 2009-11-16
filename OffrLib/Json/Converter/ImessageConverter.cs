using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Message;

namespace Offr.Json.Converter
{
    public class IMessageConverter : CanJsonConvertor<IMessage>
    {

        public override IMessage Create()
        {
            return new OfferMessage();
        }
    }
}
