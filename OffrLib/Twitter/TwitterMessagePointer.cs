using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterMessagePointer : IMessagePointer
    {
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }
        public string ProviderNameSpace { get { return "twitter"; } }
        public string ProviderMessageID { get; private set; }
        public TwitterMessagePointer()
        {
        }

        public TwitterMessagePointer(long status_id)
        {
            ProviderMessageID = status_id.ToString();
        }
    }
}
