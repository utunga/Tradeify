using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Users;

namespace Offr.Text
{
    public class TwitterUserPointer : IUserPointer, IEnhancedUserPointer
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
            return MatchTag.GetHashCode();
        }
    }
}
