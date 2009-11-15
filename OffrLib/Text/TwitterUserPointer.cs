using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Users;

namespace Offr.Text
{
    public class TwitterUserPointer : IUserPointer, IEnhancedUserPointer, IEquatable<TwitterUserPointer>
    {

        public string MatchTag { get { return ProviderNameSpace + "/" + ProviderUserName; } }
        public string ProviderUserName { get; private set; }
        public string ProviderNameSpace { get; private set; }

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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return ((TwitterUserPointer)obj).MatchTag.Equals(MatchTag);
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

    }
}
