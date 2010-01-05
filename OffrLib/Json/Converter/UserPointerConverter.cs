using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Text;

namespace Offr.Json.Converter
{
    public class UserPointerConverter : CanJsonConvertor<IUserPointer>
    {
        public override IUserPointer Create(JsonReader reader, JsonSerializer serializer)
        {
            //FIXME1need to read into the JSon decide what type of convertor to create
            //return new TwitterUserPointer();
            string type = JSON.ReadProperty<string>(serializer, reader, "type");
            if (type.Equals("OpenSocialUserPointer"))
                return new OpenSocialUserPointer();
            else if (type.Equals("TwitterUserPointer") || type.Equals("MockUserPointer"))
                return new TwitterUserPointer();
            else return new TwitterUserPointer();//throw new JsonReaderException("Failed to recognize User pointer of type:"+type);
        }
    }
}
