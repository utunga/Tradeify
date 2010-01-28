using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using NUnit.Framework;
using Offr.Demo;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.OpenSocial;
using Offr.Query;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestSerialization
    {

        IMessageRepository _messageRepository;
        TagRepository _singletonTagProvider;

        public TestSerialization()
        {
            _singletonTagProvider = new TagRepository();
            _singletonTagProvider.FilePath = "data/initial_tags.json";
            _singletonTagProvider.InitializeFromFile();
            ILocationProvider locationProvider = new MockLocationProvider();

            IMessageParser realMessageParser = new RegexMessageParser(_singletonTagProvider, locationProvider);
            _messageRepository = new MessageRepository();
            IncomingMessageProcessor incomingMessageProcessor = new IncomingMessageProcessor(_messageRepository, realMessageParser);
            IRawMessageProvider rawMessageProvider = new MockRawMessageProvider(incomingMessageProcessor); 
            rawMessageProvider.Update();
        }

        [Test]
        public void SerializeAllMessagesOut()
        {
           
            // grab all the messages
            IEnumerable<IMessage> messages = _messageRepository.AllMessages();
            List<IMessage> messagesToSend = new List<IMessage>(messages.Take(10));
            
            // serailize them 
            string serializedMessages = JSON.Serialize(messagesToSend);

            // just dump to console (this is not a 'test' as it always passes)
            Console.Out.Write(serializedMessages);
        }

        [Test]
        [Ignore("Only do this when you want to update initial_offers.json")]
        public void SerializeDemoMessagesToFile()
        {
             ILocationProvider locationProvider = new GoogleLocationProvider();
            IMessageParser realMessageParser = new RegexMessageParser(_singletonTagProvider, locationProvider);
            MessageRepository tempMessageRepository = new MessageRepository();
            IncomingMessageProcessor incomingMessageProcessor = new IncomingMessageProcessor(tempMessageRepository, realMessageParser);
            IRawMessageProvider rawMessageProvider = new DemoMessageProvider(incomingMessageProcessor);
            rawMessageProvider.Update();

            tempMessageRepository.FilePath = "../../data/demo_offers.json";
            tempMessageRepository.SerializeToFile();
        }

        [Test]
        public void TestRoundTripEachOfTheMessages()
        {
            // grab all the messages
            IEnumerable<IMessage> messages = _messageRepository.AllMessages();
            foreach (IMessage origMessage in messages)
            {
                //MockMessagePointer origPointer = origMessage.CreatedBy;
                //origMessage.CreatedBy = new TwitterUserPointer(origPointer.ProviderUserName,origPointer.);
                // serialize it 
                string serialized = JSON.Serialize(origMessage);
                Console.Out.WriteLine(serialized);
                // deserialize it
                IMessage deserializedMessage = JSON.Deserialize<IMessage>(serialized);
                //DEBUG
                Console.Out.WriteLine(JSON.Serialize(deserializedMessage));
                Assert.AreEqual(origMessage, deserializedMessage, "Something went wrong with round trip serialization");
            }
        }

        [Test]
        public void TestTwitterMessagePointerRoundTrip()
        {
            TwitterMessagePointer orig = new TwitterMessagePointer(23412123);
            string json = JSON.Serialize(orig);
            TwitterMessagePointer deserialized = JSON.Deserialize<TwitterMessagePointer>(json);
            Assert.AreEqual(orig, deserialized, "Round trip serialization for TwitterMessagePointer failed");
        }

        [Test]
        public void TestLocationRoundTrip()
        {
            Location.Location orig = new Location.Location();
            String message = JSON.Serialize(orig);
            Location.Location deserialized = JSON.Deserialize<Location.Location>(message);
            Assert.AreEqual(orig.GeoLat, deserialized.GeoLat);
            Assert.AreEqual(orig.GeoLong, deserialized.GeoLong);
            Assert.AreEqual(orig.Address, deserialized.Address);
            Assert.AreEqual(orig.Tags, deserialized.Tags);
            Assert.AreEqual(orig, deserialized, "Round trip serialization for location false");
        }

        //[Test]
        //public void TestRawMessageRoundTrip()
        //{
        //    RawMessage orig = new RawMessage();
        //    orig.Text = "Im a Raw Message.. be afraid";
        //    orig.Pointer = new TwitterMessagePointer();
        //    orig.CreatedBy = new TwitterUserPointer();
        //    orig.Timestamp = DateTime.MinValue;
        //    string message = JSON.Serialize(orig);
        //    RawMessage deserialized = JSON.Deserialize<RawMessage>(message);
        //    Assert.AreEqual(orig, deserialized, "Round trip serialization for Raw Message false");
        //}

        [Test]
        public void TwitterMessagePointerRoundTrip()
        {
            TwitterMessagePointer orig= new TwitterMessagePointer();
            string message = JSON.Serialize(orig);
            TwitterMessagePointer deserialized = JSON.Deserialize<TwitterMessagePointer>(message);
            Assert.AreEqual(orig, deserialized, "Round trip serialization for Raw Message false");
        }

        [Test]
        public void TestTagListRoundTrip()
        {
            
            TagList orig = new TagList();
            ITagRepository tagProvider = Global.Kernel.Get<ITagRepository>();
            orig.Add(tagProvider.GetAndAddTagIfAbsent("foo", TagType.tag));
            orig.Add(tagProvider.GetAndAddTagIfAbsent("freecycle", TagType.tag));
            orig.Add(tagProvider.GetAndAddTagIfAbsent("barter", TagType.tag));

            string serialized = JSON.Serialize(orig);
            TagList deserialized = JSON.Deserialize<TagList>(serialized);

            Assert.AreEqual(orig, deserialized, "Round trip serialization for TagList failed");
        }

        [Test]
        public void TestOfferMessageRoundTrip()
        {
            MockRawMessage source = new MockRawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
            OfferMessage orig = new OfferMessage();
            orig.AddThumbnail("thumb");
           // orig.Source = m;
           orig.RawText = source.Text;
           orig.Timestamp = source.Timestamp;
            orig.MessagePointer = source.Pointer;
            string json = JSON.Serialize(orig);
            Console.Out.WriteLine(json);
            OfferMessage deserialized = JSON.Deserialize<OfferMessage>(json);
            Assert.AreEqual(orig, deserialized);
        }

        [Test]
        public void TestOfferMessageList()
        {
            MockRawMessage source = new MockRawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
            OfferMessage originalMessage = new OfferMessage();
            //o.Source = m;
            originalMessage.RawText = source.Text;
            originalMessage.Timestamp = source.Timestamp;
            originalMessage.MessagePointer = source.Pointer;
            List<OfferMessage> messages = new List<OfferMessage> { originalMessage, originalMessage };
            string initialMessages = JSON.Serialize(messages);
            List<OfferMessage> initialMessageobj = JSON.Deserialize<List<OfferMessage>>(initialMessages);
            //AssertMessagesAreTheSame(messages, initialMessageobj);
            Assert.AreEqual(messages, initialMessageobj);
            Console.WriteLine(initialMessages);
        }

        private void AssertMessagesAreTheSame(List<OfferMessage> expectedList, List<OfferMessage> actualList)
        {
            Assert.That(expectedList.Count == actualList.Count);
            for (int i = 0; i < actualList.Count; i++)
            {
                OfferMessage expected = expectedList[i];
                OfferMessage actual = actualList[i];
                Assert.AreEqual(expected.Location, actual.Location, "Location was not the same");
                Assert.AreEqual(expected.OfferText, actual.OfferText, "Offer text was not the same");
                Assert.AreEqual(expected.MoreInfoURL, actual.MoreInfoURL, "MoreInfoURL was not the same");
                Assert.AreEqual(expected.LocationTags, actual.LocationTags, "LocationTags not the same");
                //Assert.AreEqual(expected.OfferedBy,actual.OfferedBy,"OfferedBy not the same");
                Assert.AreEqual(expected.Currencies, actual.Currencies, "Currencies not the same");
                Assert.AreEqual(expected.EndBy, actual.EndBy, "EndBy not the same");
                Assert.AreEqual(expected.EndByText, actual.EndByText, "EndByText not the same");
                //There is also the protected field MessageType
            }
        }
        [Test]
        public void TestUserInitialiazedCorrectly()
        {
            TwitterUserPointer original=new TwitterUserPointer();
            string twitterString = JSON.Serialize(original);
            Assert.AreEqual(original, JSON.Deserialize<TwitterUserPointer>(twitterString));
            OpenSocialUserPointer originalOS = new OpenSocialUserPointer();
            string oSString = JSON.Serialize(originalOS);
            Assert.AreEqual(originalOS, JSON.Deserialize<OpenSocialUserPointer>(oSString));
            //Moc
        }
        [Test]
        public void TestDateRoundTrip()
        {
            //"2010-01-06T02:05:51.830746Z"
            //string timestring = "\"2010-01-06T02:05:51.830746Z\"";
            //string timestring2 = "2010-01-06T02:05:51.830746Z";
            //DateTime time = DateTime.Parse(timestring2);
            DateTime time = DateTime.Now;
            Assert.AreEqual(time, JSON.Deserialize<DateTime>(JSON.Serialize(time)));
        }

        [Test]
        public void TestVariableTypeSerialization()
        {

            MessageRepository messageRepository = new MessageRepository(); // need to keep this seperate so as not to mess up other tests
            IMessageParser messageParser = new RegexMessageParser(_singletonTagProvider, new MockLocationProvider());
            IRawMessageReceiver messageReceiver = new IncomingMessageProcessor(messageRepository, messageParser);
            
            //add two messages of different types
            messageReceiver.Notify(DemoData.RawMessages[0]);
            messageReceiver.Notify(MockData.RawMessages[0]);
            var openSocialRawMessage = new OpenSocialRawMessage("ooooby", "i have vegetables available in wellington. for #free. #ooooby", "utunga", "", "");
            messageReceiver.Notify(openSocialRawMessage);

            //serialize out 
            string tempFileName = "testOffers.json";
            messageRepository.FilePath = tempFileName;
            messageRepository.SerializeToFile();

            // ok great now check that we can deserialize
            messageRepository = new MessageRepository(); // need to keep this seperate so as not to mess up other tests
            messageRepository.FilePath = tempFileName;
            messageRepository.InitializeFromFile();

            Assert.AreEqual(messageRepository.MessageCount, 3, "expected to find 3 messages after deserialization");

            //TwitterStatusXml twitterStatus = new TwitterStatusXml();
            //twitterStatus.CreatedAt = DateUtils.TwitterTimeStampFromUTCDateTime(DateTime.Now);
            //twitterStatus. = DateUtils.TwitterTimeStampFromUTCDateTime(DateTime.Now);
           
        }

    }
}