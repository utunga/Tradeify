using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json.Converter;
using Offr.Repository;

namespace Offr.Text
{
    public interface IUserPointer : ICanJson ,IEquatable<IUserPointer> , ITopic
    {
        string MatchTag { get; }
        string ProviderUserName { get; }
        string ProviderNameSpace { get; }
    }
}
