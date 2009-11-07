using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public interface IMessagePointer 
    {
        string MatchTag { get; }
        //string SourceURL { get; }
        //IResolvedURI ResolvedURL { get; }
        string ProviderMessageID { get; }
        string ProviderNameSpace { get; }
    }
}
