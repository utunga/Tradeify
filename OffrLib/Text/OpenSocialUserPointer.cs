using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;

namespace Offr.Text
{
    public class OpenSocialUserPointer :IUserPointer
    {
        public OpenSocialUserPointer(string name)
        {
            ProviderUserName = name;
            ProviderNameSpace = "Open Social";
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "provider_user_name", ProviderUserName);
            JSON.WriteProperty(serializer, writer, "provide_name_space", ProviderNameSpace);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            ProviderUserName = JSON.ReadProperty<string>(serializer, reader, "provider_user_name");
            ProviderNameSpace = JSON.ReadProperty<string>(serializer, reader, "provide_name_space");
        }

        public bool Equals(IUserPointer other)
        {
            throw new NotImplementedException();
        }

        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderUserName;  }
        }

        public string ProviderUserName { get; set; }

        public string ProviderNameSpace { get; set; }
    }
}
