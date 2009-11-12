using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageRepository
    {
        private MessageRepository _target;

        public TestMessageRepository()
        {
            //Global.Initialize(new DefaultNinjectConfig());
        }

        [Test]
        public void InitializeWithRecentOffers_BlowsUpWithWrongFile()
        {
            try
            {
                //Global.InitializeWithRecentOffers("data/typo_asdfuihsg.json"); // should be copied to bin/Debug output directory because of build action properties on that file 
                MessageRepository.InitializeMessagesFilePath = "data/typo_asdfuihsg.json";
                _target = new MessageRepository(); 
                Assert.Fail("Expected to get an  exception from trying to trying to load bad file");
            }
            catch (IOException)
            {
                //expected
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected to get an IOexception from trying to trying to load bad file, instead got:" + ex);
            }
        }

        [Test]
        public void TestInitializeWithRecentOffers_Works()
        {
            //Global.InitializeWithRecentOffers(); // should be copied to bin/Debug output directory because of build action properties on that file 
            MessageRepository.InitializeMessagesFilePath = "data/initial_offers.json";
            _target = new MessageRepository(); 
            //Assert.AreEqual(10, new List<IMessage>(_target.GetAll()).Count, "Expected to load 10 messages");
            
        }


        [Test]
        public void TestSerialize()
        {
            //NOTE2J its a drag that this is still causing errors here's what I suggest
            // can't be bothered debugging this but thinking maybe you should try out
            // JSON.Net see if it does a better job - http://james.newtonking.com/projects/json-net.aspx
            // if you decide its good - put it only the relevant dlls in the lib dir and then referecen them from the 
            // project .. ps i looked into MSofts new DataContractJsonSerializer a litte but apparently it sucks
           // JavaScriptSerializer serializer = new JavaScriptSerializer();
            MessageRepository.InitializeMessagesFilePath = "data/initial_offers.json";
            //RawMessage m = new RawMessage();
            RawMessage m=new RawMessage("",new TwitterMessagePointer(0), new TwitterUserPointer(""),DateTime.Now.ToString());
            OfferMessage o = new OfferMessage();
            o.Source = m;
            //serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<OfferMessage> messages = new List<OfferMessage> { o, o };
            //string initialMessages = serializer.Serialize(l);
            string initialMessages = JsonConvert.SerializeObject(messages);
           // List<OfferMessage> initialMessageobj = serializer.Deserialize<List<OfferMessage>>(initialMessages);
            List<OfferMessage> initialMessageobj = JsonConvert.DeserializeObject<List<OfferMessage>>(initialMessages, new RawMessageConvertor());


           AssertMessagesAreTheSame(messages, initialMessageobj, "Expected to load 10 messages");


            
            //Deserialize<List<IMessage>>(stringBuilder.ToString());
            //[{"OfferText":null,"MoreInfoURL":null,"Location":null,"OfferedBy":null,"Currencies":[],"Tags":[],"EndBy":null,"EndByText":null,"IsValid":false,"CreatedBy":null,"Tags":[],"TimeStamp":"\/Date(-62135596800000)\/","Source":null,"HashTags":[],"CommunityTags":[],"MessageType":0}]
            //{"messages":[{"offer_text":"For sale: Kitchen jug. #nzd or #barter L:Paekakariki http://bit.ly/234","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twollar_test_1","more_info_url":"http://www.twitter.com/twollar_test_1","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"nzd"},{"tag":"barter"},{"tag":"twademe"},{"tag":"household"},{"tag":"kitchen"}]},{"offer_text":"Huge pile of already split #wood will gladly #barter for whatever you have got L:Paekakariki http://bit.ly/1231 #barter #wood ...","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twollar_test_1","more_info_url":"http://www.twitter.com/twollar_test_1","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"wood"},{"tag":"barter"},{"tag":"barter"},{"tag":"wood"}]},{"offer_text":"Everything including the kitchen sink.. old wood, formica, etc. #free to a good home. L:home","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twollar_test_1","more_info_url":"http://www.twitter.com/twollar_test_1","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"free"},{"tag":"wood"},{"tag":"freecycle"}]},{"offer_text":"Big lemon tree, #free #lemons L:Paekakariki http://bit.ly/1231 #free #lemons","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twollar_test_1","more_info_url":"http://www.twitter.com/twollar_test_1","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"free"},{"tag":"lemons"},{"tag":"free"},{"tag":"lemons"},{"tag":"veges"},{"tag":"ooooby"}]},{"offer_text":"I have some beautiful big squash available for #free L:Whitehead bay, Waiheke http://bit.ly/1234","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twooooby","more_info_url":"http://www.twitter.com/twooooby","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"free"},{"tag":"ooooby"},{"tag":"veges"},{"tag":"squash"}]},{"offer_text":"Come get some of our beautiful #pumpkin #barter for other veges L:Whitehead bay, Waiheke http://bit.ly/1234 #waiheke #ooooby  ...","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twooooby","more_info_url":"http://www.twitter.com/twooooby","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"pumpkin"},{"tag":"barter"},{"tag":"ooooby"}]},{"offer_text":"Corn L:5 Wellington Road, Paekakariki for http://bit.ly/234","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twollar_test_1","more_info_url":"http://www.twitter.com/twollar_test_1","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"free"},{"tag":"corn"},{"tag":"veges"},{"tag":"ooooby"}]},{"offer_text":"Kitchen jug, works great, in L:K Road http://bit.ly/1231","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twooooby","more_info_url":"http://www.twitter.com/twooooby","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"free"},{"tag":"freecycle"},{"tag":"household"},{"tag":"kitchen"},{"tag":"jug"}]},{"offer_text":"Huge corn #free L:Waiheke http://bit.ly/234 #free","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twooooby","more_info_url":"http://www.twitter.com/twooooby","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"free"},{"tag":"free"},{"tag":"corn"},{"tag":"veges"},{"tag":"ooooby"}]},{"offer_text":"Keen to swap our old fridge for anything smaller #barter in L:Whitehead bay, Waiheke http://bit.ly/1234 #waiheke #household # ...","more_info_url":null,"date":"2009-06-24 ","user":{"screen_name":"twooooby","more_info_url":"http://www.twitter.com/twooooby","profile_pic_url":"http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg","ratings_pos_count":2,"ratings_neg_count":1,"ratings_inc_count":10},"tags":[{"tag":"barter"},{"tag":"household"}]}]}
            //[{"OfferText":"For sale: Kitchen jug. #nzd or #barter L:Paekakariki http://bit.ly/234","more_info_url":null,"date":"2009-06-24 ","MoreInfoURL":"http://www.twitter.com/twollar_test_1","Location":null,"OfferedBy":null,"Currencies":[],"Tags":[],"EndBy":null,"EndByText":null,"IsValid":false,"CreatedBy":null,"Tags":[],"TimeStamp":"\/Date(-62135596800000)\/","Source":null,"HashTags":[],"CommunityTags":[],"MessageType":0}]

Console.WriteLine(initialMessages);
           _target = new MessageRepository();
            //_target.GetAll();
            Console.WriteLine();
        }

        private void AssertMessagesAreTheSame(List<OfferMessage> expected, List<OfferMessage> actual, string errorPrefix)
        {
            //
        }
    }

    public class RawMessageConvertor : CustomCreationConverter<IRawMessage>
    {

        public override IRawMessage Create(Type objectType)
        {
            return new RawMessage();

        }

    }
}