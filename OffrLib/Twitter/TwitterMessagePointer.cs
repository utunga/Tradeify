using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterMessagePointer : MessagePointerBase 
    {
        public override string ProviderNameSpace
        {
            get { return "twitter"; }
            protected set
            {
                if (!"twitter".Equals(value))
                {
                    throw new ApplicationException("expect namespace for TwitterMessagePointer to be 'twitter' only");
                }
            }
        }

        public TwitterMessagePointer()
        {
        }

        public TwitterMessagePointer(long status_id)
        {
            ProviderMessageID = status_id.ToString();
        }

        public override string ToString()
        {
            return "TwitterMessagePointer(" + MatchTag + ")";
        }
    }
}
