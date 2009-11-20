using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json.Converter;

namespace Offr.Text
{
    public interface IUserPointer : ICanJson
    {
        string MatchTag { get; }
        string ProviderUserName { get; }
        string ProviderNameSpace { get; }
    }
}
