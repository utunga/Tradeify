using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Offr.Text;
using Offr.Users;

namespace Offr.Message
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
          //offers: [
          //  { "date": "12/12/2009",
          //      "offer_text": "Hey someting",
          //      "more_info_url": "http://foobar.com/html",
          //      "tags": [{ "tag": "foo" }, { "tag": "bar" }, { "tag": "baz"}],
          //      user: {
          //          "screen_name": "utunga",
          //          "profile_pic_url": "/images/foo.jpg",
          //          "more_info_url": "http://www.foo.bar.com/",
          //          "ratings_pos_count": 2,
          //          "ratings_neg_count": 0,
          //          "ratings_inc_count": 12
          //      }
          //  },
          //  { "date": "12/12/2009",
          //      "offer_text": "Hey sometingq",
          //      "more_info_url": "http://foobar.com/html",
          //      "tags": [{ "tag": "fxxoo" },{ "tag":  "bddar"},{ "tag":  "baz"}],
          //      user: {
          //          "screen_name": "shelly",
          //          "profile_pic_url": "/images/foo.jpg",
          //          "more_info_url": "http://www.foo.bar.com/",
          //          "ratings_pos_count": 4,
          //          "ratings_neg_count": 2,
          //          "ratings_inc_count": 1
          //      }
          //  }

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
                            dict = new Dictionary<string, object>() {
                                {"offer_text", offer.OfferText}, 
                                {"more_info_url", offer.MoreInfoURL},
                                {"date", offer.Source.Timestamp},
                            };

                            Dictionary<string, object> userDict = new Dictionary<string, object>() 
                            {
                                {"screen_name", offer.CreatedBy.ProviderUserName}
                            };
                            if (offer.CreatedBy is IEnhancedUserPointer)
                            {
                                userDict.Add("more_info_url", ((IEnhancedUserPointer)offer.CreatedBy).MoreInfoUrl);
                                userDict.Add("profile_pic_url", ((IEnhancedUserPointer)offer.CreatedBy).ProfilePicUrl);
                                userDict.Add("ratings_pos_count", FAKE_POS_COUNT);
                                userDict.Add("ratings_neg_count", FAKE_NEG_COUNT);
                                userDict.Add("ratings_inc_count", FAKE_INC_COUNT);
                            }
                            dict.Add("user", userDict);

                            List<Dictionary<string, object>> tags = new List<Dictionary<string, object>>();
                            foreach (ITag tag in offer.Tags)
                            {
                                tags.Add(new Dictionary<string, object>()
                                {
                                    {"tag", tag.tag}
                                });
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
            throw  new NotSupportedException("SOrry this is a one way converter currently");
        }

    }
}
