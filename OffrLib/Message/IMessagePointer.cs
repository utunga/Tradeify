using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json.Converter;
using Offr.Text;

namespace Offr.Message
{
    public interface IMessagePointer:ICanJson
    {
        string MatchTag { get; }
        //string SourceURL { get; }
        //IResolvedURI ResolvedURL { get; }
        string ProviderMessageID { get; }
        string ProviderNameSpace { get; }
    }
}
