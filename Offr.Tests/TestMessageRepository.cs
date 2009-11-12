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

            MessageRepository.InitializeMessagesFilePath = "data/initial_offers.json";
            RawMessage m = new RawMessage("", new TwitterMessagePointer(0), new TwitterUserPointer(""), DateTime.Now.ToString());
            OfferMessage o = new OfferMessage();
            o.Source = m;
            List<OfferMessage> messages = new List<OfferMessage> { o, o };

            string initialMessages = JsonConvert.SerializeObject(messages);
            List<OfferMessage> initialMessageobj = JsonConvert.DeserializeObject<List<OfferMessage>>(initialMessages, new RawMessageConvertor());

            AssertMessagesAreTheSame(messages, initialMessageobj, "Expected to load 10 messages");
            Console.WriteLine(initialMessages);
            _target = new MessageRepository();


            //serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });

        }

        private void AssertMessagesAreTheSame(List<OfferMessage> expectedList, List<OfferMessage> actualList, string errorPrefix)
        {
            Assert.That(expectedList.Count==actualList.Count);
            for (int i = 0; i < actualList.Count; i++)
            {
                OfferMessage expected = expectedList[i];
                OfferMessage actual = actualList[i];
                Assert.AreEqual(expected.Location,actual.Location,"Location was not the same");
                Assert.AreEqual(expected.OfferText,actual.OfferText, "Offer text was not the same");
                Assert.AreEqual(expected.MoreInfoURL, actual.MoreInfoURL, "MoreInfoURL was not the same");
                Assert.AreEqual(expected.LocationTags,actual.LocationTags,"LocationTags not the same");
                Assert.AreEqual(expected.OfferedBy,actual.OfferedBy,"OfferedBy not the same");
                Assert.AreEqual(expected.Currencies,actual.Currencies,"Currencies not the same");
                Assert.AreEqual(expected.EndBy,actual.EndBy,"EndBy not the same");
                Assert.AreEqual(expected.EndByText,actual.EndByText,"EndByText not the same");
                //There is also the protected field MessageType
            }
        }
    }
    //needed to avoid the serializer throwing an error when trying to instantialize an IRawMessage interface!
    public class RawMessageConvertor : CustomCreationConverter<IRawMessage>
    {

        public override IRawMessage Create(Type objectType)
        {
            return new RawMessage();

        }

    }
}