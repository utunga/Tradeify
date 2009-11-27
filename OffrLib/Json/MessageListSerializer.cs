using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Offr.Message;
using Offr.Text;
using Offr.Users;

namespace Offr.Json
{
    public class MessageListSerializer : JavaScriptConverter
    {
        public const int FAKE_POS_COUNT = 2;
        public const int FAKE_NEG_COUNT = 1;
        public const int FAKE_INC_COUNT = 10;
                                
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
                        case MessageType.offr_test:
                        case MessageType.offr:
                            OfferMessage offer = (OfferMessage) msg;
                            /* A dictionary is made up of:
                             * "offer_text"
                             * "more_info_url"
                             *"date"
                             * A User:A dictionary also contains a userDict which has a
                             *      "screen_name"
                             *          and if the offer is created by an enhanced user
                             *          more_info_url"
                             *          "ratings_pos_count"
                             *          "ratings_neg_count
                             *          "ratings_inc_count"
                             * A tags: A dictionary also contains a list of tags that arent locations or msg types
                             * 
                             * After you are done with each dictionary add it to your message list 
                             * 
                             * Once you are done with the message list add it to a new dictionary under the key messages
                             */
                            dict = new Dictionary<string, object>() {
                                                                        {"offer_text", offer.OfferText}, 
                                                                        {"more_info_url", offer.MoreInfoURL},
                                                                        {"date", offer.FriendlyTimeStamp },
                                                                    };

                            Dictionary<string, object> userDict = new Dictionary<string, object>() 
                                                                      {
                                                                          {"screen_name", offer.CreatedBy.ProviderUserName}
                                                                      };
                            if (offer.CreatedBy is IEnhancedUserPointer)
                            {
                                userDict.Add("more_info_url", ((IEnhancedUserPointer)offer.CreatedBy).MoreInfoUrl);
                                userDict.Add("profile_pic_url", ((IEnhancedUserPointer)offer.CreatedBy).ProfilePicUrl);
                                //userDict.Add("ratings_pos_count", FAKE_POS_COUNT);
                                //userDict.Add("ratings_neg_count", FAKE_NEG_COUNT);
                                //userDict.Add("ratings_inc_count", FAKE_INC_COUNT);
                            }
                            dict.Add("user", userDict);

                            List<Dictionary<string, object>> tags = new List<Dictionary<string, object>>();
                            foreach (ITag tag in offer.Tags)
                            {
                                if ((tag.type != TagType.msg_type) &&
                                    (tag.type != TagType.loc))
                                {
                                    tags.Add(new Dictionary<string, object>()
                                                 {
                                                     {"tag", tag.tag}
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

    }
}