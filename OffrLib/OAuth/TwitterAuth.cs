using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Offr.Twitter;
using Offr.Users;

namespace Offr.OAuth
{
    public static class TwitterAuth
    {
        const string AUTH_KEY = "oauth";
        const string USER_KEY = "oauth";

        public static void StoreSession(OAuthTwitter twitterAuth)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session[AUTH_KEY] = twitterAuth;

                //We now have the credentials, so make a call to the Twitter API.

                string url = "http://twitter.com/account/verify_credentials.xml";
                string xml = twitterAuth.oAuthWebRequest(OAuthTwitter.Method.GET, url, String.Empty);
                User currentUser = XmlTwitterParser.ParseUser(xml);
                if (currentUser != null)
                {
                    HttpContext.Current.Session[USER_KEY] = currentUser;
                }
            }
        }

        public static bool HasValidSession()
        {
            return ((HttpContext.Current != null) &&
                    (HttpContext.Current.Session[AUTH_KEY] != null) &&
                    (HttpContext.Current.Session[USER_KEY] != null));
        }

        public static OAuthTwitter CurrentSession
        {
            get 
            {
                if ((HttpContext.Current != null) &&
                (HttpContext.Current.Session[AUTH_KEY] != null))
                {
                    return (OAuthTwitter)HttpContext.Current.Session[AUTH_KEY];
                }
                return null;
            }
        }

        public static User CurrentUser
        {
            get
            {
                if ((HttpContext.Current != null) &&
                    (HttpContext.Current.Session[USER_KEY] != null))
                {
                    return (User)HttpContext.Current.Session[USER_KEY];
                }
                return null;
            }
        }

    }
}
