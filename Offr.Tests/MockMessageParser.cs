using System;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    public class MockMessageParser : IMessageParser
    {
        public IMessage Parse(IRawMessage source)
        {
            if (!(source is MockRawMessage))
            {
                throw new ApplicationException("MockSourceText only knows how to deal with MockSourceText");
            }

            // mockRawMessage saves you the trouble of parsing because - hey! its already parsed!
            MockRawMessage mockRaw = (MockRawMessage) source;
            OfferMessage msg = new OfferMessage();
            msg.CreatedBy = mockRaw.CreatedBy;
            msg.Location = mockRaw.Location;
            msg.MoreInfoURL = mockRaw.MoreInfoURL;
            msg.CreatedBy = mockRaw.CreatedBy;
            msg.OfferText = mockRaw.OfferText;
            if (mockRaw.EndBy.HasValue)
            {
                msg.SetEndBy(mockRaw.EndByText, mockRaw.EndBy.Value);
            }
            else
            {
                msg.ClearEndBy();
            }
           
            foreach (ITag tag in mockRaw.Tags)
            {
                msg.AddTag(tag);
            }
            msg.IsValid = true;

            
            //if (CONVERT_MOCK_TO_REAL)
            //{
            //    RawMessage realRawMessage = new RawMessage(source.Text, source.Pointer, source.CreatedBy, source.Timestamp);
            //    msg.Source = realRawMessage;
            //}

            //FIXME1 this all has to go - don't keep the source around!
            //msg.Source = mockRaw;
            msg.RawText = mockRaw.Text;
            msg.Timestamp = mockRaw.Timestamp;
            msg.MessagePointer = mockRaw.Pointer;
            return msg;

        }
    }
}