using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Offr.Location;
using Offr.Repository;
using Offr.RSS;
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

            BaseMarketMessage msg;
            switch (GetMessageType(sourceText, tags))
            {
                case MessageType.wanted:
                    msg = new WantedMessage();
                    break;

                case MessageType.offer:
                default:
                    msg = new OfferMessage();
                    break;
            }

            msg.CreatedBy = rawMessage.CreatedBy;
            msg.Timestamp = rawMessage.Timestamp;
            msg.MessagePointer = rawMessage.Pointer;
            msg.RawText = rawMessage.Text;
            bool containsGroup = false;
            foreach (ITag tag in tags)
            {
                if (tag.Type == TagType.msg_type) continue; //skip messages of this Type
                if(tag.Type==TagType.group) containsGroup = true;
                msg.AddTag(tag);
            }
            
            if (!containsGroup)
            {
                ITag possibleGroup = CheckForAtSymbolGroup(sourceText);
                if(possibleGroup != null)
                {
                    //remove the @ooooby tag from the messages
                    sourceText=sourceText.Replace("@" + possibleGroup.Text, "");
                    msg.AddTag(possibleGroup);
                }
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

            string untilText;
            DateTime until;
            if (ParseUntil(sourceText, out untilText, out until))
            {
                msg.SetEndBy(untilText, until);
            }
            msg.AddThumbnail(GetImageUrl(sourceText));
            return msg;
        }


        private ITag CheckForAtSymbolGroup(string sourceText)
        {
            List<ITag> groups = TagProvider.GetGroups();
            foreach (ITag tag in groups)
            {
                Regex re = new Regex("@"+tag.Text, RegexOptions.IgnoreCase);
                Match matchGroup = re.Match(sourceText);
                if (matchGroup.Groups.Count >= 1 && !matchGroup.Groups[0].Value.Equals(""))
                {
                    return tag;
                }
            }
            return null;
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
            Regex reIn = new Regex("in (([^#|^\\.])*)", RegexOptions.IgnoreCase);
            Match matchIn = reIn.Match(sourceText);
            if (matchIn.Groups.Count >= 1)
            {
                string afterL = matchIn.Groups[1].Value;
                //string address = afterL;
                //address = address.TrimStart(" in ".ToCharArray());
                result = _locationProvider.ParseFromApproxText(afterL);
            }
            if (result != null) return result;

            Regex reOpenIn = new Regex(" in (([^#])*)", RegexOptions.IgnoreCase);
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
                ITag tag = _tagProvider.GetTagIfExists(tagString, TagType.tag);
                yield return tag ?? new Tag(TagType.tag, tagString);

            }
        }

        private string SubstituteTagIfSubstituteExists(string s)
        {
            if (s.Equals("wants", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("iwant", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("want", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("wanting", StringComparison.OrdinalIgnoreCase))
                return MessageType.wanted.ToString();

            if (s.Equals("offering", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("ioffer", StringComparison.OrdinalIgnoreCase))
                return MessageType.offer.ToString();

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

        private bool ParseUntil(string offerText, out string untilText, out DateTime until)
        {
            Regex EndBy = new Regex(" until (.*)", RegexOptions.IgnoreCase);
            Match matchEndBy = EndBy.Match(offerText);
            string candidateText = matchEndBy.Groups[1].Value;
            if (matchEndBy.Groups.Count >= 1)
            {
                while (candidateText.Length >= 1)
                {
                    DateTime result;
                    if (DateTime.TryParse(candidateText, out result))
                    {
                        untilText = candidateText;
                        until = result;
                        return true;
                    }
                    Regex endRegex = new Regex("([ |,][^( |,)]+$$)");
                    Match endMatch = endRegex.Match(candidateText);
                    if (endMatch.Groups.Count > 1)
                    {
                        candidateText = candidateText.TrimEnd(endMatch.Groups[0].Value.ToCharArray());
                        candidateText = candidateText.Trim();
                    }
                    else break;
                }
            }
            untilText = "";
            until = DateTime.MaxValue;
            return false;
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
        private MessageType GetMessageType(string offerText, IEnumerable<ITag> tags)
        {
            offerText = offerText.Trim();
            //string REGEX = @"([\w]+\s+){" + 0 + "}";
            string[] words = Regex.Split(offerText, @"\s+");

            if (words.Length<2) return MessageType.offer;
            string firstWord = words[0].ToLowerInvariant();
            string secondWord = words[1].ToLowerInvariant();

            if (firstWord.Contains("offer"))
            {
                return MessageType.offer;
            }
            else if (firstWord.Contains("want"))
            {
                return MessageType.wanted;
            }
            else if (secondWord.Contains("offer"))
            {
                return MessageType.offer;
            }
            else if (secondWord.Contains("want"))
            {
                return MessageType.wanted;
            }

            //otherwise look for "#offer" / "#wanted" tags
            foreach (ITag tag in tags)
            {
                if (tag.Text.Equals(MessageType.offer.ToString())) return MessageType.offer;
                else if (tag.Text.Equals(MessageType.wanted.ToString())) return MessageType.wanted;
            }
            //Default to offer for now?
            return MessageType.offer;
        }

        #endregion

        #region Test
#if DEBUG
        public ILocation TEST_GetLocation(string offerText)
        {
            return ParseLocation(offerText);
        }
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
        public DateTime? TEST_ParseUntil(string offerText)
        {
            DateTime until;
            string untilText;
            if (ParseUntil(offerText, out untilText, out until))
            {
                return until;
            }
            return null;
        }
        public MessageType TEST_GetMessageType(string offerText, IEnumerable<ITag> tags)
        {
            return GetMessageType(offerText, tags);
        }
#endif
        #endregion Test
    }
}
