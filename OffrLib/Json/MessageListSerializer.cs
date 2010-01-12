using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using NLog;
using Offr.Message;
using Offr.Text;
using Offr.Users;

namespace Offr.Json
{
    public class MessageListSerializer : JavaScriptConverter
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
                                
        public override IEnumerable<Type> SupportedTypes
        {
            //Define the ListItemCollection as a supported type.
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(List<IMessage>) })); }
        }
       
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IEnumerable<IMessage> messages = obj as IEnumerable<IMessage>;

            if (messages != null)
            {
                // Create the representation.
                Dictionary<string, object> result = new Dictionary<string, object>();
                ArrayList messagesList = new ArrayList();
                foreach (IMessage msg in messages)
                {
                    Dictionary<string, object> dict;
                    switch(msg.MessageType)
                    {
                        case MessageType.offr:
                            OfferMessage offer = (OfferMessage) msg;
          
                            dict = new Dictionary<string, object>() {
                                                                        {"offer_text", TruncateSourceText(offer)}, 
                                                                        {"more_info_url", offer.MoreInfoURL},
                                                                        {"thumbnail_url",offer.Thumbnail},
                                                                        {"date", offer.FriendlyTimeStamp },
                                                                        {"offer_latitude",offer.Location.GeoLat},
                                                                        {"offer_longitude",offer.Location.GeoLong}
                                                                    };

                            Dictionary<string, object> userDict = new Dictionary<string, object>() 
                                                                      {
                                                                          {"screen_name", offer.CreatedBy.ProviderUserName}
                                                                      };
                            if (offer.CreatedBy is IEnhancedUserPointer)
                            {
                                userDict.Add("more_info_url", ((IEnhancedUserPointer)offer.CreatedBy).MoreInfoUrl);
                                string pic = offer.Thumbnail ?? ((IEnhancedUserPointer)offer.CreatedBy).ProfilePicUrl;
                                userDict.Add("profile_pic_url", pic);
                            }
                            dict.Add("user", userDict);

                            List<Dictionary<string, object>> tags = new List<Dictionary<string, object>>();
                            foreach (ITag tag in offer.Tags)
                            {
                                if ((tag.Type != TagType.msg_type))
                                {
                                    tags.Add(new Dictionary<string, object>()
                                                 {
                                                     {"tag", tag.Text},
                                                     {"type", tag.Type.ToString()}
                                                 });
                                }
                            }
                            dict.Add("tags", tags);
                            
                            messagesList.Add(dict);
                            break;
                    }
                }
                result["messages"] = messagesList;
                return result;
            }
            // blank return, shouldn't happen
            return new Dictionary<string, object>();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw  new NotSupportedException("Sorry this is a one way converter currently");
        }

        // remove any tags added to end 
        // of the source text and the tag "#offr" from the front (if its at the front)
        // to get just the actual 'offer' part of the text
        private string TruncateSourceText(IOfferMessage offer)
        {
            string offerText = "" + offer.OfferText;
            
            // drop the 'offer' tag
            //FIXME need 'tag aliases' already
            offerText = offerText.Replace("#offr", "");
            offerText = offerText.Replace("#offer", "");
            offerText = offerText.Replace("#ihave", "");

            // drop the 'offer' tag
            if (offer.MoreInfoURL != null)
            {
                offerText = offerText.Replace(offer.MoreInfoURL, "");
            }

            offerText = offerText.Trim();

            //look for a hash followed by any set of characters followed by the end of file character
            Regex re = new Regex("(#[a-zA-Z0-9_]+$$)");
            Match match = re.Match(offerText);
            //repeat until no more are found
            int loopCheck = 0;
            while (match.Groups.Count > 1)
            {
                string tag = match.Groups[0].Value;
                offerText = offerText.TrimEnd(tag.ToCharArray());
                offerText = offerText.Trim();
                match = re.Match(offerText);
                if (loopCheck++ > 1000)
                {
                    _log.Error("Something terrible went wrong with truncateSourceText whilst trying to parse " + offerText);
                    break;
                }
            }
            return offerText;
        }
    }
}