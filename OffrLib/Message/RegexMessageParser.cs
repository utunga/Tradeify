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
            msg.Source = source;

            foreach (ITag tag in ParseTags(source))
            {
                //NOTE2J not msg.Tags.Add(tag); 
                // because that would violate the 'law of two dots' aka 'the law of demeter' (google it)
                if (tag.type == TagType.msg_type) continue; //skip messages of this type
                msg.AddTag(tag);
            }

            ILocation location = ParseLocation(source.Text);
            if (location!=null)
            {
                msg.Location = location;
                foreach (ITag s in location.Tags)
                {
                    msg.AddTag(s);
                }
            }

            msg.OfferText = TruncateSourceText(msg.Tags, source.Text);

            msg.IsValid = true;
            return msg;
        }

        #endregion

        #region private helper methods

        private ILocation ParseLocation(string sourceText)
        {
            // 1. parse out the l:address: bit
            // 2. give it to the LocationProvider and get a Location back
            // 3. add 'location' tags to the message for all the tags in the location

            Regex re = new Regex("l:([^:]+):", RegexOptions.IgnoreCase);
            Match match = re.Match(sourceText);
            if (match.Groups.Count > 1)
            {
                //grab the first part of the address
                string address = match.Groups[1].Value;
                _locationProvider.Parse(address);
            }
            return null;
        }

        private IEnumerable<ITag> ParseTags(IRawMessage source)
        {
            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(source.Text);
            
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                tagString = tagString.Replace("#", "");
          
                TagType type = GuessMessageType(tagString);
                yield return _tagProvider.FromTypeAndText(type, tagString);
            }
        }
        
        private static TagType GuessMessageType(string tagText)
        {
            //NOTE2J please do the following
            // 1. move this code into the tag provider (change signature of _tagProvider.FromTypeAndText(type, tagString);
            //    to _tagProvider.GetTag(tagString); 
            // 2. rather than guessing the tag type based on this set list
            //    do the following:
            //      i. check to see if the tag string has already been assigned
            //     ii. if so return that tag, which will therefore have the specified type
            //    iii. if not, create a new tag of type 'tag' and add to the list of known tags
            //     iv. in the constructor for TagProvider, create a list of special per the switch (and the loop below)
            //      v. later, (or if you get time) we will serialize this list of special/defined tags from an init file
            //     vi. even later, we will replace the init file with a proper database back end (ie TagRepository)

            foreach (MessageType msgType in Enum.GetValues(typeof(MessageType)))
            {                
                if (msgType.ToString().Equals(tagText))
                {
                    // for messages where we flag the type using a hash tag "eg #offr"
                    return TagType.msg_type;
                }
            } 
            
            switch (tagText.ToLowerInvariant())
            {
                case "ooooby":
                case "freecycle":
                    return TagType.group;

                case "cash only":
                case "cash":
                case "nzd":
                case "barter":
                case "free":
                    return TagType.type;

                default:
                    return TagType.tag;
            }
        }

        private string TruncateSourceText(IList<ITag> tags, string sourceText)
        {
            // NOTE2J please take code below and get it to work here and make it less ugly
            // preferably make it a lot shorter and maybe even readable would be nice
            // also perhaps use the passed in list of tags instead of re-doing the regex?

            // basically what we need to do is remove any tags added to end 
            // of the source text and the tag "#offr" from the front (if its at the front)
            // to get just the actual 'offer' part of the text


            //FIXME 
            //PROFUSE APOLOGIES FOR THIS AWFUL CODE BUT WANTED TO HACK TOGETHER SOMETHING TO WORK FOR DEMO

            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(sourceText);

            SortedList<int, string> justAwfulCode = new SortedList<int, string>();
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                string restOfLine = sourceText.Substring(sourceText.IndexOf(tagString));
                MatchCollection restOfLineResults = re.Matches(restOfLine);

                string tagsInRestOfLine = "";
                foreach (Match match2 in restOfLineResults)
                {
                    tagsInRestOfLine += match2.Groups[0].Value + " ";
                }
                tagsInRestOfLine = tagsInRestOfLine.Trim();

                if (tagsInRestOfLine.Length == restOfLine.Length)
                {
                    // why then, its all tags from here on! 
                    string startOfLine = sourceText.Substring(0, sourceText.Length - restOfLine.Length - 1);
                    justAwfulCode.Add(startOfLine.Length, startOfLine);
                }
            }

            string offerText = sourceText;
            foreach (string startOfLine in justAwfulCode.Values)
            {
                offerText = startOfLine;
                break;
            }

            foreach (MessageType messageType in Enum.GetValues(typeof(MessageType)))
            {
                string messageTypeTag = "#" + messageType.ToString() + " "; // add the " " because otherwise #offr_test gets cut off to be come "_test"
                if (offerText.StartsWith(messageTypeTag))
                {
                    offerText = offerText.Substring((messageTypeTag).Length - 1);
                    offerText = offerText.Trim();
                }
            }

            //END OF PROFUSE APOLOGIES
            return offerText;
        }

        #endregion
    }
}
