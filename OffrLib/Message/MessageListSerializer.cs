using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Offr.Message
{
   public class MessageListSerializer : JavaScriptConverter
    {

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

                            userDict.Add("user", userDict);
                            
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
