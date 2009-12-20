using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Users;

namespace Offr.Text
{
    public class OpenSocialUserPointer :IEnhancedUserPointer
    {
        public string ProviderUserName { get; set; }

        public string ProviderNameSpace { get; set; }

        public string ProfilePicUrl { get; set; }

        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderUserName; }
        }

        public string ScreenName
        {
            get { return ProviderUserName;  }
        }

        public string MoreInfoUrl
        {
            get { throw new System.NotImplementedException("Need to implement where on open social/oooby the user pointer is at"); }
        }

        public OpenSocialUserPointer(string nameSpace, string name, string profilePicUrl)
        {
            ProviderNameSpace = nameSpace;
            ProviderUserName = name;
            ProfilePicUrl = profilePicUrl;
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "provider_user_name", ProviderUserName);
            JSON.WriteProperty(serializer, writer, "provide_name_space", ProviderNameSpace);
            JSON.WriteProperty(serializer, writer, "profile_pic_url", ProfilePicUrl);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            ProviderUserName = JSON.ReadProperty<string>(serializer, reader, "provider_user_name");
            ProviderNameSpace = JSON.ReadProperty<string>(serializer, reader, "provide_name_space");
            ProfilePicUrl = JSON.ReadProperty<string>(serializer, reader, "profile_pic_url");
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return ((obj is IUserPointer) ? this.Equals((IUserPointer)obj) : false);
        }

        public bool Equals(IUserPointer userPointer)
        {
            if (userPointer == null)
            {
                return false;
            }
            return Equals(MatchTag, userPointer.MatchTag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ProviderUserName != null ? ProviderUserName.GetHashCode() : 0);
                result = (result*397) ^ (ProviderNameSpace != null ? ProviderNameSpace.GetHashCode() : 0);
                return result;
            }
        }

    }
}
