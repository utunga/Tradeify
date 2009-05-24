using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Offr.Text;

namespace Offr.Message
{
    public class RegexMessageParser : IMessageParser
    {
        readonly ITagProvider _tagProvider;
        public RegexMessageParser(ITagProvider tagProvider)
        {
            _tagProvider = tagProvider;
        }

        public static List<string> GetTags(string sourceText, out string offerText)
        {

            Regex re = new Regex("(#[a-zA-Z0-9_]+)");
            MatchCollection results = re.Matches(sourceText);
            //string marray = results[0].Groups[1].Value;

            List<String> values = new List<String>();
            foreach (Match match in results)
            {
                //what the heck is the groups thing anyway?
                string tagString = match.Groups[0].Value;
                tagString = tagString.Replace("#", "");
                //tagString = tagString.Replace("_", " ");
                values.Add(tagString);
            }

            //FIXME 
            //PROFUSE APOLOGIES FOR THIS AWFUL CODE BUT WANTED TO HACK TOGETHER SOMETHING TO WORK FOR DEMO
            SortedList<int, string> justAwfulCode = new SortedList<int, string>();
            foreach (Match match in results)
            {
                string tagString = match.Groups[0].Value;
                string restOfLine = sourceText.Substring(sourceText.IndexOf(tagString));
                MatchCollection restOfLineResults = re.Matches(restOfLine);

                int lengthOfTagsInRestOfLine = 0;
                foreach (Match match2 in restOfLineResults)
                {
                    lengthOfTagsInRestOfLine += match2.Groups[0].Value.Length;
                }

                if (lengthOfTagsInRestOfLine == restOfLine.Length)
                {
                    // why then, its all tags from here on! 
                    string startOfLine = sourceText.Substring(0, sourceText.Length - restOfLine.Length-1);
                    justAwfulCode.Add(startOfLine.Length, startOfLine);
                }
            }

            offerText = sourceText;
            foreach (string startOfLine in justAwfulCode.Values)
            {
                offerText = startOfLine;
                break;
            }

            //END OF PROFUSE APOLOGIES

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
                TagType type = GuessType(tagString);
                if (type == TagType.msg_type) { continue; } //skip messages of this type
                ITag tag = _tagProvider.FromTypeAndText(type, tagString);
                msg.Tags.Add(tag);
            }
            msg.OfferText = offerText;

            msg.IsValid = true;
            return msg;
        }

        private static TagType GuessType(string tagText)
        {
            
            foreach (MessageType msgType in Enum.GetValues(typeof(MessageType)))
            {                
                if (msgType.ToString().Replace("_", " ").Equals(tagText))
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

                case "nz":
                case "auckland":
                case "wellington":
                case "paekakariki":
                case "waiheke":
                    return TagType.loc;

                default:
                    return TagType.tag;
            }
        }
    }
}
