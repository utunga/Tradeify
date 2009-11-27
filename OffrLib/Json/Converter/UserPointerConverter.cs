using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Text;

namespace Offr.Json.Converter
{
    public class UserPointerConverter : CanJsonConvertor<IUserPointer>
    {
        public override IUserPointer Create(JsonReader reader)
        {
            return new TwitterUserPointer();
        }
    }
}
