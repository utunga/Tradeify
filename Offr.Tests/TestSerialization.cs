using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

using NUnit.Framework;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestSerialization
    {

        public TestSerialization()
        {
            Global.Initialize(new TestModule());
        }

        [Test]
        public void TestSerializeAllMessages()
        {

            IMessageProvider provider = Global.Kernel.Get<IMessageProvider>();
            IEnumerable<IMessage> messages = provider.AllMessages;
     
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Register the custom converter.
            serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });

            List<IMessage> messagesToSend = new List<IMessage>(messages.Take(10));
            //Console.Out.Write(serializer.Serialize(messagesToSend));
            Console.Out.Write(JSON.Serialize(messagesToSend));
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
        public void TestOfferMessageRoundTrip()
        {
            RawMessage m = new RawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
            OfferMessage orig = new OfferMessage();
            orig.AddThumbnail("thumb");
            orig.Source = m;
            string json = JSON.Serialize(orig);
            Console.Out.WriteLine(json);
            OfferMessage deserialized = JSON.Deserialize<OfferMessage>(json);
            Assert.AreEqual(orig, deserialized);
        }

        [Test]
        public void TestOfferMessageList()
        {
            RawMessage m = new RawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
            OfferMessage o = new OfferMessage();
            o.Source = m;
            List<OfferMessage> messages = new List<OfferMessage> { o, o };
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