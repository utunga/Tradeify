using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

using NUnit.Framework;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.Query;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestSerialization
    {

        MessageProvider _messageProvider;

        public TestSerialization()
        {
            // load the mock messages via this NonMockRawMessageProvider madness
            NonMockRawMessageProvider rawMessageProvider = new NonMockRawMessageProvider();
            TagProvider singletonTagProvider = new TagProvider();
            GoogleLocationProvider locationProvider = new GoogleLocationProvider();
            RegexMessageParser realMessageParser = new RegexMessageParser(singletonTagProvider, locationProvider);
            _messageProvider = new MessageProvider(rawMessageProvider, realMessageParser);
            _messageProvider.Update();
        }

        [Test]
        public void SerializeAllMessagesOut()
        {
           
            // grab all the messages
            IEnumerable<IMessage> messages = _messageProvider.AllMessages;
            List<IMessage> messagesToSend = new List<IMessage>(messages.Take(10));
            
            // serailize them 
            string serializedMessages = JSON.Serialize(messagesToSend);

            // just dump to console (this is not a 'test' as it always passes)
            Console.Out.Write(serializedMessages);
        }


        [Test]
        public void TestRoundTripEachOfTheMessages()
        {
            // grab all the messages
            IEnumerable<IMessage> messages = _messageProvider.AllMessages;

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

        [Test]
        public void TestRawMessageRoundTrip()
        {
            RawMessage orig = new RawMessage("Im a Raw Message.. be afraid", new TwitterMessagePointer(), new TwitterUserPointer(), DateTime.MinValue.ToString());
            string message = JSON.Serialize(orig);
            RawMessage deserialized = JSON.Deserialize<RawMessage>(message);
            Assert.AreEqual(orig, deserialized, "Round trip serialization for Raw Message false");
        }
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
            ITagProvider tagProvider = Global.Kernel.Get<ITagProvider>();
            orig.Add(tagProvider.GetTag("foo", TagType.tag));
            orig.Add(tagProvider.GetTag("freecycle", TagType.tag));
            orig.Add(tagProvider.GetTag("barter", TagType.tag));

            string serialized = JSON.Serialize(orig);
            TagList deserialized = JSON.Deserialize<TagList>(serialized);

            Assert.AreEqual(orig, deserialized, "Round trip serialization for TagList failed");
        }

        [Test]
        public void TestOfferMessageRoundTrip()
        {
            RawMessage source = new RawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
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
            RawMessage source = new RawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
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
        public void TestMockDataSerialization()
        {
            //Moc
        }
    }
}