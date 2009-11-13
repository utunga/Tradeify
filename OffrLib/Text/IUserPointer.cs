using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public interface IUserPointer
    {
        string MatchTag { get; }
        string ProviderUserName { get; }
        string ProviderNameSpace { get; }
    }
}
