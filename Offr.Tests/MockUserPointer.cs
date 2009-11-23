using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Text;
using Offr.Users;

namespace Offr.Tests
{
    public class MockUserPointer : IUserPointer, IEnhancedUserPointer, IEquatable<MockUserPointer>
    {
        public string MatchTag { get { return ProviderNameSpace + "/" + ProviderUserName; } }
        public string ProviderUserName { get; private set; }
        public string ProviderNameSpace { get; private set; }
        public MockUserPointer(string providerNameSpace, string providerUserID)
        {
            ProviderNameSpace = providerNameSpace;
            ProviderUserName = providerUserID;
        }

        #region Implementation of IEnhancedUserPointer

        public string ProfilePicUrl { get; set; }
        public string ScreenName { get; set; }
        public string MoreInfoUrl
        {
            get { return "http://test/" + ProviderUserName; }
        }

        #endregion

        public bool Equals(MockUserPointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ProviderUserName, ProviderUserName) && Equals(other.ProviderNameSpace, ProviderNameSpace) && Equals(other.ProfilePicUrl, ProfilePicUrl) && Equals(other.ScreenName, ScreenName);
        }
        /* for testing purposes*/
        public bool Equals(TwitterUserPointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ProviderUserName, ProviderUserName) && Equals(other.ProviderNameSpace, ProviderNameSpace) && Equals(other.ProfilePicUrl, ProfilePicUrl) && Equals(other.ScreenName, ScreenName);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (IUserPointer)) return false;
            if (obj.GetType() != typeof(TwitterUserPointer)) return Equals((TwitterUserPointer)obj);
            else return Equals((MockUserPointer)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (ProviderUserName != null ? ProviderUserName.GetHashCode() : 0);
                result = (result*397) ^ (ProviderNameSpace != null ? ProviderNameSpace.GetHashCode() : 0);
                result = (result*397) ^ (ProfilePicUrl != null ? ProfilePicUrl.GetHashCode() : 0);
                result = (result*397) ^ (ScreenName != null ? ScreenName.GetHashCode() : 0);
                return result;
            }
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
           TwitterUserPointer twitterUserPointer= new TwitterUserPointer(ProviderUserName,
               ProfilePicUrl,ProviderNameSpace, ScreenName);
            twitterUserPointer.WriteJson(writer,serializer);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
