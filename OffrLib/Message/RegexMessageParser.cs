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

        public IMessage Parse(IRawMessage rawMessage)
        {
            OfferMessage msg = new OfferMessage();
            msg.CreatedBy = rawMessage.CreatedBy;
            ////msg.Source = source; //Remove this
            msg.Timestamp = rawMessage.Timestamp;
            msg.MessagePointer = rawMessage.Pointer;
            msg.RawText = rawMessage.Text;

            string sourceText = rawMessage.Text; 
            foreach (ITag tag in ParseTags(sourceText))
            {
                if (tag.Type == TagType.msg_type) continue; //skip messages of this Type
                msg.AddTag(tag);
            }
            //source.Pointer.
            ILocation location = ParseLocation(sourceText);
            if (location != null)
            {
                msg.Location = location;
                foreach (ITag s in location.Tags)
                {
                    msg.AddTag(s);
                }
            }
            msg.OfferText = sourceText;
            msg.IsValid = true;
            msg.MoreInfoURL = GetMoreInfoUrl(sourceText);
            msg.AddThumbnail(GetImageUrl(sourceText));
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
            Regex re = new Regex("[ l| L]:([^:]+):", RegexOptions.IgnoreCase);
            Match match = re.Match(sourceText);
            if (match.Groups.Count > 1)
            {
                //grab the first part of the address
                string address = match.Groups[1].Value;
                return _locationProvider.Parse(address);
            }

            GoogleLocationProvider locationProvider = _locationProvider as GoogleLocationProvider;

            Regex reOpenEnd = new Regex("(( l:| L:)([^:]+))", RegexOptions.IgnoreCase);
            Match matchOpenEnd = reOpenEnd.Match(sourceText);
            if (matchOpenEnd.Groups.Count > 1)
            {
                string afterL = match.Groups[3].Value;
                string address = afterL;
                return locationProvider.ParseFromApproxText(address);
            }

            Regex reIn = new Regex(" in .*$$", RegexOptions.IgnoreCase);
            Match matchIn = reIn.Match(sourceText);
            if (matchIn.Groups.Count >= 1)
            {
                string afterL = match.Groups[0].Value;
                string address = afterL;
                address = address.TrimStart(" in ".ToCharArray());
                return locationProvider.ParseFromApproxText(address);
            }

            // didn't find any location
            return null;
        }

        //private ILocation LocationWithIn(string sourceText)
        //{
        //    // snippet of code from parsing of content after "In" which was inexplicably 
        //    // differnet from approach used when after L:
        //    //    //search for a location then trim the array one character at a time until you reach the ':' character
        //    //while (address.Length >= 1)
        //    //{
        //    //    //Regex re = new Regex("(#[a-zA-Z0-9_]+$$)");
        //    //    ILocation newLocation = Parse(address);
        //    //    if (newLocation != null && newLocation.Accuracy != null)
        //    //    {
        //    //        if (best == null) best = newLocation;
        //    //        if (newLocation.Accuracy >= best.Accuracy) best = newLocation;
        //    //    }
        //    //    Regex endRegex = new Regex("([ |,][^( |,)]+$$)");
        //    //    Match endMatch = endRegex.Match(address);
        //    //    if (endMatch.Groups.Count > 1)
        //    //    {
        //    //        address = address.TrimEnd(endMatch.Groups[0].Value.ToCharArray());
        //    //        address = address.Trim();
        //    //    }
        //    //    else break;
        //    //    //address = address.Substring(0, address.Length - 2);
        //    //}
        //    //return best;
        //}
       
        private IEnumerable<ITag> ParseTags(string sourceText)
        {
            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(sourceText);
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                tagString = tagString.Replace("#", "");
                yield return _tagProvider.GetAndAddTagIfAbsent(tagString,TagType.tag);
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

        #endif
        #endregion Test
    }
}
