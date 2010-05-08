using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json.Converter;
using Offr.Text;

namespace Offr.Message
{
    public interface IMessagePointer : ICanJsonObject, IEquatable<IMessagePointer>
    {
        //string SourceURL { get; }
        //IResolvedURI ResolvedURL { get; }
        string ProviderMessageID { get; }
        string ProviderNameSpace { get; }
        string MatchTag { get; }
    }
}
