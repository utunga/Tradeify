using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Message
{
    public class WantedMessage:BaseMarketMessage
    {
        public static string HASHTAG = "#" + MessageType.wanted;
        public override MessageType MessageType
        {
            get { return MessageType.wanted; }
        }
    }
}
