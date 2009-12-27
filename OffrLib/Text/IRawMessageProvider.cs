using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{ 
    public interface IRawMessageProvider  
    {
        string ProviderNameSpace { get; }
        void Update();
        //IEnumerable<IRawMessage> ForQueryText(string query);
        //IRawMessage ByID(string providerMessageID);
    }
}
