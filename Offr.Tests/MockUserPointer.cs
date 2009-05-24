using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
