using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Text;
using Offr.Users;

namespace Offr.Tests
{
    public class MockUserPointer : IUserPointer, IEnhancedUserPointer
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
