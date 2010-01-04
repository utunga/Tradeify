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
            //FIXME1need to read into the JSon decide what type of convertor to create
            //return new TwitterUserPointer();
            return new OpenSocialUserPointer();
        }
    }
}
