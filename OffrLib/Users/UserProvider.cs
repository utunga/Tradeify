using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;
using Offr.Common;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Users
{
    public class UserProvider : IUserProvider, IMemCache
    {
        [Inject]
        public WebRequest.WebRequestMethod RetrieveWebContent { get; set; }

        //fixme should use a proper cache thing
        private Dictionary<IUserPointer, User> _users;
        private readonly object[] _syncLock = new object[0];

        public UserProvider()
        {
            Invalidate();
        }

   
        public User FromPointer(IUserPointer userPointer)
        {
             lock (_syncLock)
             {

                 if (_users.ContainsKey(userPointer))
                 {
                     return (_users[userPointer]);
                 }
             }

            User user = null;
            switch (userPointer.ProviderNameSpace)
            {
                case "twitter":
                    user = RetrieveTwitterUser((TwitterUserPointer)userPointer);
                    break;

                default:
                    throw new NotSupportedException("userPointer.ProviderNameSpace not supported yet");
            }

            if (user != null)
            {
                lock (_syncLock)
                {
                    _users[userPointer] = user; // last in wins is OK (in case of race condition,t hat is)
                }
            }

            return  user;
        }

        public void Invalidate()
        {
            _users = new Dictionary<IUserPointer, User>();
        }

        private User RetrieveTwitterUser(TwitterUserPointer pointer)
        {
            //FIXME the problem with this is rapid using up of rate limiting.. grr
            string url = String.Format(TwitterRawMessageProvider.TWITTER_USER_URI, pointer.ProviderUserName);
            string xml = RetrieveWebContent(url);
            User user = XmlTwitterParser.ParseUser(xml);
            return user;
        }
    }
}
