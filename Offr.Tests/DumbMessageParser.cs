using System;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    public class DumbMessageParser : IMessageParser
    {
        public IMessage Parse(IRawMessage source)
        {

            OfferMessage msg = new OfferMessage();
            msg.CreatedBy = source.CreatedBy;
            msg.Source = source;
            msg.OfferText = source.Text;

            msg.IsValid = true;
            return msg;

            // rest of this would require an actual parser, so, no can do for now!
            //   msg.Location = source.Location;
            //msg.MoreInfoURL = mockRaw.MoreInfoURL;
            //msg.CreatedBy = mockRaw.OfferedBy;
            //if (mockRaw.EndBy.HasValue)
            //{
            //    msg.SetEndBy(mockRaw.EndByText, mockRaw.EndBy.Value);
            //}
            //else
            //{
            //    msg.ClearEndBy();
            //}
            //foreach (ITag tag in mockRaw.Tags)
            //{
            //    msg.Tags.Add(tag);
            //}
        }
    }
}