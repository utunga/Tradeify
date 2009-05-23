using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Tests
{
    public class MockUserPointer : IUserPointer
    {
        public string MatchTag { get { return ProviderNameSpace + "/" + ProviderUserName; } }
        public string ProviderUserName { get; private set; }
        public string ProviderNameSpace { get; private set; }
        public MockUserPointer(string providerNameSpace, string providerUserID)
        {
            ProviderNameSpace = providerNameSpace;
            ProviderUserName = providerUserID;
        }
    }
}
