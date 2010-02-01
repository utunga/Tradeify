using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public class WantedMessage:BaseMarketMessage
    {
        public override MessageType MessageType
        {
            get { return MessageType.wanted; }
        }

        public WantedMessage() : base()
        {
            base.AddTag(new Tag(TagType.msg_type, MessageType.wanted.ToString()));
        }
    }
}
