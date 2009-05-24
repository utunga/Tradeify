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
            msg.Source = mockRaw;
            foreach (ITag tag in mockRaw.Tags)
            {
                msg.Tags.Add(tag);
            }
            msg.IsValid = true;
            return msg;

        }
    }
}