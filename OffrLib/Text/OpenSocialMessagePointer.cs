using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Message;

namespace Offr.Text
{
    public class OpenSocialMessagePointer:IMessagePointer
    {
        public OpenSocialMessagePointer(string id)
        {
            ProviderMessageID = id;
        }

        public bool Equals(IMessagePointer other)
        {
            throw new NotImplementedException();
        }

        public string ProviderMessageID
        {
             get; private set; 
        }

        public string ProviderNameSpace
        {
            get { return "Open Social"; }
        }

        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "provider_name_space", ProviderNameSpace);
            JSON.WriteProperty(serializer, writer, "message_id", ProviderMessageID); ;
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            JSON.ReadProperty<string>(serializer, reader, "provider_name_space");
            this.ProviderMessageID = JSON.ReadProperty<string>(serializer, reader, "message_id");
        }
    }
}
