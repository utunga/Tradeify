using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Message;

namespace Offr.Text
{
    public class OpenSocialMessagePointer : MessagePointerBase
    {

        public override string ProviderNameSpace
        {
            get; 
            protected set;
        }

        internal OpenSocialMessagePointer()
        {
        }
        public OpenSocialMessagePointer(string providerNameSpace)
        {
            ProviderNameSpace = providerNameSpace;
            ProviderMessageID = Guid.NewGuid().ToString();//FIXME
        }

    }
}
