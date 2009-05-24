using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Offr.Text;

namespace Offr.Message
{
    public class RegexParser : IMessageParser
    {
        readonly ITagProvider _tagProvider;
        public RegexParser(ITagProvider tagProvider)
        {
            _tagProvider = tagProvider;
        }

        public static List<string> GetTags(string sourceText, out string offerText)
        {

            offerText = sourceText;
            return new List<string>();

            Regex re = new Regex("(#[a-zA-Z0-9]+)");
            MatchCollection results = re.Matches(sourceText);
            //string marray = results[0].Groups[1].Value;

            List<String> values = new List<String>();
            foreach (Match match in results)
            {
                //what the heck is the groups thing anyway?
                values.Add(match.Groups[0].Value);
            }
            offerText = "rest";
            return values;
        }


        public IMessage Parse(IRawMessage source)
        {

            OfferMessage msg = new OfferMessage();
            msg.CreatedBy = source.CreatedBy;
            msg.Source = source;

            string offerText;
            List<string> stringTags = GetTags(source.Text, out offerText);
            foreach (string tagString in stringTags)
            {
                ITag tag = _tagProvider.FromString(tagString); // will just classify all as of TagType.tag if you don't specify a prefix
                msg.Tags.Add(tag);
            }
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
          
        }
    }
}
