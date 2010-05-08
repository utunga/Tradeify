using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Offr.Message;
using Offr.RSS;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Json.JObjectConverter
{
    public class MessagePointerConverter : CanJObjectConverter<IMessagePointer>
    {

        public override IMessagePointer Create(JObject jObject, JsonSerializer serializer)
        {
            //string type = JSON.ReadProperty<string>(serializer, reader, "type");
            //For backwards compatibles
            //JObject jObject = JObject.Load(reader);
            //string type = jObject["type"].Value<string>();

            string type = JSON.ReadProperty<string>(jObject, "type");
            if (type != null)
            {
                switch (type)
                {
                    case "TwitterMessagePointer": //type.Equals(typeof(TwitterMessagePointer).GetType().Name:
                        return new TwitterMessagePointer();

                    case "RSSMessagePointer":
                        return new RSSMessagePointer();

                    case "OpenSocialMessagePointer":
                        return new OpenSocialMessagePointer();

                    default: //slower, needed for tests
                        return (IMessagePointer)Assembly.GetExecutingAssembly().CreateInstance(type);
                }
            }
            else
            {
                string nameSpace = JSON.ReadProperty<string>(jObject, "provider_name_space");
                switch (nameSpace)
                {
                    case "twitter":
                        return new TwitterMessagePointer();
                    default:
                        return new OpenSocialMessagePointer();
                }
            }
        }
    }
}