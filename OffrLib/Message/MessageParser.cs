using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public class MessageParser: IMessageParser
    {
       
        public IMessage Parse(IRawMessage source)
        {
            MessageType type = GetMessageType(source);
            switch (type)
            {
                case MessageType.offr_test:
                case MessageType.offr:
                    OfferMessage offer = new OfferMessage();
                    //offer.Source = source;
                    //ParseIntoOffer(offer, source.Text);
                    //// needs internal access to the OfferMessage class, hence put this in the same package
                    
                    break;

            }
            throw new NotImplementedException("wah");
        }

        private static MessageType GetMessageType(IRawMessage source)
        {
            throw new NotImplementedException();
        }
    }
}
