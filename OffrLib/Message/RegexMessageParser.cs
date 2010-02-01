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
        private readonly ITagRepository _tagProvider;
        private readonly ILocationProvider _locationProvider;
        public
        ITagRepository TagProvider
    {
        get
        {
            return _tagProvider;
        }
    } 

        public RegexMessageParser(ITagRepository tagProvider, ILocationProvider locationProvider)
        {
            _tagProvider = tagProvider;
            _locationProvider = locationProvider;
        }

        #region the main method

        public IMessage Parse(IRawMessage rawMessage)
        {
            string sourceText = rawMessage.Text;
            IEnumerable<ITag> tags = ParseTags(sourceText, rawMessage);
            MessageType type = this.GetMessageType(sourceText, tags);
            BaseMarketMessage msg=null;
            if (type == MessageType.wanted) msg = new WantedMessage();
            else if (type == MessageType.offer) msg = new OfferMessage();
            msg.CreatedBy = rawMessage.CreatedBy;
            ////msg.Source = source; //Remove this
            msg.Timestamp = rawMessage.Timestamp;
            msg.MessagePointer = rawMessage.Pointer;
            msg.RawText = rawMessage.Text;                        
            foreach (ITag tag in tags)
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
            msg.MessageText = sourceText;
            msg.MoreInfoURL = GetMoreInfoUrl(sourceText);
            msg.SetEndBy("", GetEndByInfo(sourceText));
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
            ILocation result = null;

            //Look for 'l:' followed by one or more of any character except ':' followed by ':'
            Regex re = new Regex("[ l| L]:([^:]+):", RegexOptions.IgnoreCase);
            Match matchStrictL = re.Match(sourceText);
            if (matchStrictL.Groups.Count > 1)
            {
                //grab the first part of the address
                string address = matchStrictL.Groups[1].Value;
                result = _locationProvider.Parse(address);
            }

            if (result != null) return result;

            Regex reOpenEnd = new Regex("(( l:| L:)([^:]+))", RegexOptions.IgnoreCase);
            Match matchOpenEnd = reOpenEnd.Match(sourceText);
            if (matchOpenEnd.Groups.Count > 1)
            {
                string afterL = matchOpenEnd.Groups[3].Value;
                string address = afterL;
                result = _locationProvider.ParseFromApproxText(address);
            }

            if (result != null) return result;
            //in  bounded by "."    
            Regex reIn = new Regex(" in (.*)\\.", RegexOptions.IgnoreCase);
            Match matchIn = reIn.Match(sourceText);
            if (matchIn.Groups.Count >= 1)
            {
                string afterL = matchIn.Groups[1].Value;
                //string address = afterL;
                //address = address.TrimStart(" in ".ToCharArray());
                result = _locationProvider.ParseFromApproxText(afterL);
            }
            if (result != null) return result;

            Regex reOpenIn = new Regex(" in (.*)", RegexOptions.IgnoreCase);
            Match matchOpenIn = reOpenIn.Match(sourceText);
            if (matchOpenIn.Groups.Count >= 1)
            {
                string afterL = matchOpenIn.Groups[1].Value;
                result = _locationProvider.ParseFromApproxText(afterL);
            }
            if (result != null) return result;

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

        private IEnumerable<ITag> ParseTags(string sourceText, IRawMessage message)
        {
            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(sourceText);
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                tagString = tagString.Replace("#", "");
                tagString = SubstituteTagIfSubstituteExists(tagString);
                //Dont do this only for message parsing!
                /*
                if(!(message is TextWrapperRawMessage))
                    yield return _tagProvider.GetAndAddTagIfAbsent(tagString, TagType.tag);
                else
                {
                 */
                    ITag tag = _tagProvider.GetTagIfExists(tagString, TagType.tag);
                    yield return tag ?? new Tag(TagType.tag, tagString);
                
            }
        }

        private string SubstituteTagIfSubstituteExists(string s)
        {
            if (s.Equals("Wants", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("Wanted", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("Wanting", StringComparison.OrdinalIgnoreCase))
                return "Want";
            else if (s.Equals("Offering", StringComparison.OrdinalIgnoreCase)) return "Offer";
            return s;
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

        private DateTime? GetEndByInfo(string offerText)
        {
            Regex EndBy = new Regex(" until (.*)", RegexOptions.IgnoreCase);
            Match matchEndBy = EndBy.Match(offerText);
            string untilText = matchEndBy.Groups[1].Value;
            if (matchEndBy.Groups.Count >= 1)
            {
                while (untilText.Length >= 1)
                {
                    DateTime result;
                    if (DateTime.TryParse(untilText, out result))
                    {
                        return result;
                    }
                    Regex endRegex = new Regex("([ |,][^( |,)]+$$)");
                    Match endMatch = endRegex.Match(untilText);
                    if (endMatch.Groups.Count > 1)
                    {
                        untilText = untilText.TrimEnd(endMatch.Groups[0].Value.ToCharArray());
                        untilText = untilText.Trim();
                    }
                    else break;
                }
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
        private MessageType GetMessageType(string offerText,IEnumerable<ITag> tags)
        {
            offerText=offerText.Trim();
            string REGEX = @"([\w]+\s+){" + 1 + "}";
            string firstWord= Regex.Match(offerText, REGEX).Value;
            firstWord = firstWord.Trim();
            firstWord = firstWord.ToLowerInvariant();
            if (firstWord.Contains("offer"))
            {
                _tagProvider.GetAndAddTagIfAbsent("offer", TagType.msg_type);
                return MessageType.offer;
            }

            else if (firstWord.Contains("want"))
            {
                _tagProvider.GetAndAddTagIfAbsent("want", TagType.msg_type);
                return MessageType.wanted;
            }
                
            foreach(ITag tag in tags)
            {
                if (tag.Text.Equals("offer")) return MessageType.offer;
                else if(tag.Text.Equals("want")) return MessageType.wanted;
            }
            //Default to offer for now?
            return MessageType.offer;
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
        public DateTime? TEST_GetEndByInfo(string offerText)
        {
            return GetEndByInfo(offerText);
        }
        public MessageType TEST_GetMessageType(string offerText, IEnumerable<ITag> tags)
        {
            return GetMessageType(offerText,tags);
        }
#endif
        #endregion Test
    }
}
