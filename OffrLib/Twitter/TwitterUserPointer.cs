using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Users;

namespace Offr.Text
{
    public class TwitterUserPointer : IUserPointer, IEnhancedUserPointer
    {
        #region fields
        public string MatchTag { get { return ProviderNameSpace + "/" + ProviderUserName; } }
        public string ProviderUserName { get; set; }
        public string ProviderNameSpace { get; set; }
        public string ID
        {
            get
            {
                return MatchTag;               
            }
        }
        // extra properties added to twitter user pointer that save us having to look up from the user provider when we have results from the search provider
        public string ProfilePicUrl { get; set; }
        public string ScreenName { get; set; }
        public string MoreInfoUrl
        {
            get { return string.Format("http://www.twitter.com/{0}", ProviderUserName); }
        }
        #endregion fields

        #region constructor
        public TwitterUserPointer() { }
        public TwitterUserPointer(string twitterScreenName)
        {
            //nb: you have to use screen name, not id, as twitter user_id's differ between search and main API's
            ScreenName = twitterScreenName; 
            ProviderUserName = twitterScreenName;
            ProviderNameSpace = "twitter";
        }

        /// <summary>
        /// initalize from the TwitterStatus from_user, profile_image_url fields
        /// </summary>
        /// <param name="from_user"></param>
        /// <param name="profile_image_url"></param>
        public TwitterUserPointer(string from_user, string profile_image_url) : this (from_user)
        {
            ProfilePicUrl = profile_image_url;
        }

        #endregion Constructor

        #region Equals
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
            return Equals(MatchTag,userPointer.MatchTag);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (MatchTag != null ? MatchTag.GetHashCode() : 0);
                return result;
            }
        }
        #endregion Equals

        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "type", "TwitterUserPointer");
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
        #endregion JSON
    }
}
