﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Users;

namespace Offr.Text
{
    public class OpenSocialUserPointer : IUserPointer,IEnhancedUserPointer
    {
        public string ProviderUserName { get; set; }

        public string ProviderNameSpace { get; set; }

        public string ProfilePicUrl { get; set; }
 
        public string MoreInfoUrl { get; set; }
 
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderUserName; }
        }
        public string ID
        {
            get
            {
                return MatchTag;
            }
        }
        public string ScreenName
        {
            get { return ProviderUserName;  }
        }

        public OpenSocialUserPointer()
        {

        }
        public OpenSocialUserPointer(string nameSpace, string name, string profilePicUrl, string profileUrl)
        {
            ProviderNameSpace = nameSpace;
            ProviderUserName = name;
            ProfilePicUrl = profilePicUrl;
            MoreInfoUrl = profileUrl;
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "type", "OpenSocialUserPointer");
            JSON.WriteProperty(serializer, writer, "provider_user_name", ProviderUserName);
            JSON.WriteProperty(serializer, writer, "provide_name_space", ProviderNameSpace);
            JSON.WriteProperty(serializer, writer, "profile_pic_url", ProfilePicUrl);
            JSON.WriteProperty(serializer, writer, "profile_url", MoreInfoUrl);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            ProviderUserName = JSON.ReadProperty<string>(serializer, reader, "provider_user_name");
            ProviderNameSpace = JSON.ReadProperty<string>(serializer, reader, "provide_name_space");
            ProfilePicUrl = JSON.ReadProperty<string>(serializer, reader, "profile_pic_url");
            MoreInfoUrl = JSON.ReadProperty<string>(serializer, reader, "profile_url");
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
                int result = (MatchTag != null ? MatchTag.GetHashCode() : 0);
                result = (result * 397) ^ (MatchTag != null ? MatchTag.GetHashCode() : 0);
                return result;
            }
        }

    }
}
