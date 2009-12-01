using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Offr.Location;
using Offr.Repository;
using Offr.Text;

namespace Offr.Message
{
    public class RegexMessageParser : IMessageParser
    {
        readonly ITagRepository _tagProvider;
        readonly ILocationProvider _locationProvider;

        public RegexMessageParser(ITagRepository tagProvider, ILocationProvider locationProvider)
        {
            _tagProvider = tagProvider;
            _locationProvider = locationProvider;
        }

        #region the main method
        public IMessage Parse(IRawMessage source)
        {
            OfferMessage msg = new OfferMessage();
            msg.CreatedBy = source.CreatedBy;
            //msg.Source = source; //Remove this
            msg.RawText = source.Text;
            msg.Timestamp = source.Timestamp;
            msg.MessagePointer = source.Pointer;
            foreach (ITag tag in ParseTags(source))
            {
                if (tag.Type == TagType.msg_type) continue; //skip messages of this Type
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
            msg.OfferText = source.Text;
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
            Regex re = new Regex("[l|L]:([^:]+):", RegexOptions.IgnoreCase);
            Match match = re.Match(sourceText);
            ILocation location = null;
            if (match.Groups.Count > 1)
            {
                //grab the first part of the address
                string address = match.Groups[1].Value;
                location = _locationProvider.Parse(address);
            }
            else return NonStrictParseLocation(sourceText);
            return location;
        }

        private ILocation NonStrictParseLocation(string sourceText)
        {
            Regex re = new Regex("((l:|L:)([^:]+))", RegexOptions.IgnoreCase);
            Match match = re.Match(sourceText);
            ILocation best = null;
            if (match.Groups.Count > 1)
            {
                string afterL = match.Groups[3].Value;
                string address = afterL;
                //search for a location then trim the array one character at a time until you reach the ':' character
                while (address.Length >= 1)
                {
                    //Regex re = new Regex("(#[a-zA-Z0-9_]+$$)");
                    ILocation newLocation = _locationProvider.Parse(address);
                    if (newLocation != null && newLocation.Accuracy != null)
                    {
                        if (best == null) best = newLocation;
                        if (newLocation.Accuracy >= best.Accuracy) best = newLocation;
                    }
                    Regex endRegex = new Regex("([ |,][^( |,)]+$$)");
                    Match endMatch = endRegex.Match(address);
                    if (endMatch.Groups.Count > 1)
                    {
                        address = address.TrimEnd(endMatch.Groups[0].Value.ToCharArray());
                        address = address.Trim();
                    }
                    else break;
                    //address = address.Substring(0, address.Length - 2);
                }
            }
            else return LocationWithIn(sourceText);
            //return the location with the best accuracy
            return best;
    }
        private ILocation LocationWithIn(string sourceText)
        {
            Regex re = new Regex(" in .*$$", RegexOptions.IgnoreCase);
            Match match = re.Match(sourceText);
            ILocation best = null;
            if (match.Groups.Count >= 1)
            {
                string afterL = match.Groups[0].Value;
                string address = afterL;
                address = address.TrimStart(" in ".ToCharArray());
                //search for a location then trim the array one character at a time until you reach the ':' character
                while (address.Length >= 1)
                {
                    //Regex re = new Regex("(#[a-zA-Z0-9_]+$$)");
                    ILocation newLocation = _locationProvider.Parse(address);
                    if (newLocation != null && newLocation.Accuracy != null)
                    {
                        if (best == null) best = newLocation;
                        if (newLocation.Accuracy >= best.Accuracy) best = newLocation;
                    }
                    Regex endRegex = new Regex("([ |,][^( |,)]+$$)");
                    Match endMatch = endRegex.Match(address);
                    if (endMatch.Groups.Count > 1)
                    {
                        address = address.TrimEnd(endMatch.Groups[0].Value.ToCharArray());
                        address = address.Trim();
                    }
                    else break;
                    //address = address.Substring(0, address.Length - 2);
                }
            }
            return best;

        }
        private IEnumerable<ITag> ParseTags(IRawMessage source)
        {
            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(source.Text);
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                tagString = tagString.Replace("#", "");
                yield return _tagProvider.GetTag(tagString,TagType.tag);
            }
        }

        private string GetMoreInfoUrl(string offerText)
        {
            String moreInfoUrl = @"http://([^\s]+[^(\.jpg)|(\.png)|(\.gif)])\s";
            Regex re = new Regex(moreInfoUrl);
            Match match = re.Match(offerText);
            if (match.Groups.Count > 1)
            {
                return "http://" + match.Groups[1].Value.Trim();
            }
            return null;
        }

        private string GetImageUrl(string offerText)
        {
            const string imageURL = @"http://([^\s]+)\.((jpg)|(png)|(gif))";
            Regex re = new Regex(imageURL);
            Match match = re.Match(offerText);
            if (match.Groups.Count > 1)
            {
                return match.Groups[0].Value.Trim();
            }

            const string twitPic = @"twitpic.com/([^\s]+)";
            re = new Regex(twitPic);
            match = re.Match(offerText);
            if (match.Groups.Count > 1)
            {
                return "http://twitpic.com/show/thumb/" + match.Groups[1].Value.Trim();
            }

            return null;
        }
        #endregion

        #region Test
        #if DEBUG
        //Test accessors
        public string TEST_GetMoreInfoUrl(string offerText)
        {
            return GetMoreInfoUrl(offerText);
        }

        //Test accessors
        public string TEST_GetImageUrl(string offerText)
        {
            return GetImageUrl(offerText);
        }
        public ILocation TEST_GetNonStrictLocation(string offerText)
        {
            return NonStrictParseLocation(offerText);
        }
        #endif
        #endregion Test
    }
}
