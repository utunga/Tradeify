using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Users;

namespace Offr.Text
{
    public class TwitterUserPointer : IUserPointer, IEnhancedUserPointer, IEquatable<TwitterUserPointer>
    {

        public string MatchTag { get { return ProviderNameSpace + "/" + ProviderUserName; } }
        public string ProviderUserName { get; set; }
        public string ProviderNameSpace { get; set; }

        // extra properties added to twitter user pointer that save us having to look up from the user provider when we have results from the search provider
        public string ProfilePicUrl { get; set; }
        public string ScreenName { get; set; }
        public string MoreInfoUrl
        {
            get { return string.Format("http://www.twitter.com/{0}", ProviderUserName); }
        }
        public TwitterUserPointer() { }
        public TwitterUserPointer(string twitterScreenName)
        {
            //nb: you have to use screen name, not id, as twitter user_id's differ between search and main API's
            ProviderUserName = twitterScreenName;
            ProviderNameSpace = "twitter/";
        }

        public TwitterUserPointer(string ProviderUserName,
               string ProfilePicUrl,string ProviderNameSpace, string ScreenName)
        {
            this.ProviderUserName = ProviderUserName;
            this.ProfilePicUrl = ProfilePicUrl;
            this.ProviderNameSpace = ProviderNameSpace;
            this.ScreenName = ScreenName;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return Equals(MatchTag,((IUserPointer)obj).MatchTag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ProviderUserName != null ? ProviderUserName.GetHashCode() : 0);
                result = (result * 397) ^ (ProviderNameSpace != null ? ProviderNameSpace.GetHashCode() : 0);
                result = (result * 397) ^ (ProfilePicUrl != null ? ProfilePicUrl.GetHashCode() : 0);
                result = (result * 397) ^ (ScreenName != null ? ScreenName.GetHashCode() : 0);
                return result;
            }
        }

        public bool Equals(TwitterUserPointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ProviderUserName, ProviderUserName) &&
                   Equals(other.ProviderNameSpace, ProviderNameSpace) &&
                   Equals(other.ProfilePicUrl, ProfilePicUrl) &&
                   Equals(other.ScreenName, ScreenName) &&
                   Equals(other.MoreInfoUrl, MoreInfoUrl) &&
                   Equals(other.MatchTag, MatchTag);
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "provider_user_name", ProviderUserName);
            JSON.WriteProperty(serializer, writer, "provide_name_space", ProviderNameSpace);
            JSON.WriteProperty(serializer, writer,"profile_pic_url",ProfilePicUrl);
            JSON.WriteProperty(serializer, writer, "screen_name", ScreenName);
            JSON.WriteProperty(serializer, writer, "more_info_url", MoreInfoUrl);
            JSON.WriteProperty(serializer, writer, "match_tag", MatchTag);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            ProviderUserName = JSON.ReadProperty<string>(serializer, reader, "provider_user_name");
            ProviderNameSpace = JSON.ReadProperty<string>(serializer, reader, "provide_name_space");
            ProfilePicUrl = JSON.ReadProperty<string>(serializer, reader, "profile_pic_url");
            ScreenName = JSON.ReadProperty<string>(serializer, reader, "screen_name");
            JSON.ReadProperty<string>(serializer, reader, "more_info_url");
            JSON.ReadProperty<string>(serializer, reader, "match_tag");
        }
    }
}
