using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public class RegexMessageParser : IMessageParser
    {
        readonly ITagProvider _tagProvider;
        readonly ILocationProvider _locationProvider;

        public RegexMessageParser(ITagProvider tagProvider, ILocationProvider locationProvider)
        {
            _tagProvider = tagProvider;
            _locationProvider = locationProvider;
        }

        #region the main method
        public IMessage Parse(IRawMessage source)
        {
            OfferMessage msg = new OfferMessage();
            msg.CreatedBy = source.CreatedBy;
            msg.Source = source; //Remove this
            foreach (ITag tag in ParseTags(source))
            {
                if (tag.type == TagType.msg_type) continue; //skip messages of this type
                msg.AddTag(tag);
            }
            //source.Pointer.
            ILocation location = ParseLocation(source.Text);
            if (location != null)
            {
                msg.Location = location;
                foreach (ITag s in location.Tags)
                {
                    msg.AddTag(s);
                }
            }
            msg.OfferText = TruncateSourceText(msg.Tags, source.Text);
            msg.IsValid = true;
            msg.MoreInfoURL = GetMoreInfoUrl(source.Text);
            msg.AddThumbnail(GetImageUrl(source.Text));
            return msg;
        }

        #endregion

        #region private helper methods

        // 1. parse out the l:address: bit
        // 2. give it to the LocationProvider and get a Location back
        // 3. add 'location' tags to the message for all the tags in the location
        private ILocation ParseLocation(string sourceText)
        {
            //Look for 'l:' followed by one or more of any character except ':' followed by ':'
            Regex re = new Regex("l:([^:]+):", RegexOptions.IgnoreCase);
            Match match = re.Match(sourceText);
            ILocation location = null;
            if (match.Groups.Count > 1)
            {
                //grab the first part of the address
                string address = match.Groups[1].Value;
                location = _locationProvider.Parse(address);
            }
            return location;
        }

        private IEnumerable<ITag> ParseTags(IRawMessage source)
        {
            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(source.Text);
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                tagString = tagString.Replace("#", "");
                yield return _tagProvider.GetTag(tagString);
            }
        }

        // remove any tags added to end 
        // of the source text and the tag "#offr" from the front (if its at the front)
        // to get just the actual 'offer' part of the text
        private string TruncateSourceText(IEnumerable<ITag> tags, string sourceText)
        {
            string offerText = sourceText;
            //trim just in case there is whitespace at the start of the message or at the end of the message that will screw up the regex
            offerText = offerText.Trim();
            offerText.TrimStart("#offr".ToCharArray());
            //look for a hash followed by any set of characters followed by the end of file character
            Regex re = new Regex("(#[a-zA-Z0-9_]+$$)");
            Match match = re.Match(offerText);
            //repeat until no more are found
            while (match.Groups.Count > 1)
            {
                string tag = match.Groups[0].Value;
                offerText = offerText.TrimEnd(tag.ToCharArray());
                offerText = offerText.Trim();
                match = re.Match(offerText);
            }
            return offerText;
        }

        private string GetMoreInfoUrl(string offerText)
        {
            String moreInfoUrl = @"http://([^\s]+)[^(\.jpg)|(\.png)|(\.gif)]+\s";
            Regex re = new Regex(moreInfoUrl);
            Match match = re.Match(offerText);
            if (match.Groups.Count > 1)
            {
                return match.Groups[0].Value.Trim();
            }
            return null;
        }
        private string GetImageUrl(string offerText)
        {
            String imageURL = @"http://([^\s]+)\.((jpg)|(png)|(gif))";
            Regex re = new Regex(imageURL);
            // Regex re = new Regex("(http://([^\.]+[(\.jpg)(\.png)(\.gif)])");
            Match match = re.Match(offerText);
            if (match.Groups.Count > 1)
            {
                return match.Groups[0].Value;
            }
            return null;
        }
        #endregion


#if DEBUG
        //Test accessors
        public string TEST_GetMoreInfoUrl(string offerText)
        {
            return GetMoreInfoUrl(offerText);
        }
#endif

    }
}
