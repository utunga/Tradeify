using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestRegexParser
    {
        IMessageParser _target;
        public TestRegexParser()
        {
            Global.Initialize(new TestRegexTestModule());
            _target = Global.Kernel.Get<IMessageParser>(); 
        }

        [Test]
        public void TestRegexMethodCleverTest()
        {
            //NOTETOFIN: you might want to screw with the MockData.RawMessages aray to get a more friendly test set..
            MockMessageParser mockParser = new MockMessageParser();
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                IOfferMessage messageWant = (OfferMessage) mockParser.Parse(rawMessage);
                IOfferMessage messageGet = (OfferMessage) _target.Parse(rawMessage);
                // actually this will certainly fail even for any kinda regex parser i can think of doing in a short time
                Assert.AreEqual(messageWant.Tags.Count, messageGet.Tags.Count, "Expect count of tags to be the same for message " + rawMessage);

                // this is the 'rest of it' bit
                Assert.AreEqual(messageWant.OfferText, messageGet.OfferText, "Expect count of tags to be the same for message " + rawMessage);

            }
        }

        [Test]
        public void TestRegexMethodSimpleTest()
        {
          
            MockRawMessage raw = new MockRawMessage(0)
            {
                Timestamp = "10pm",
                CreatedBy = MockData.User0,
                Location = MockData.Location0,
                MoreInfoURL = "http://bit.ly/message0Info",
                OfferText = "#offr_test mulch available now in L:Paekakariki for #free #barter http://bit.ly/message0Info #mulch",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.tag, "free"));
            raw.Tags.Add(new Tag(TagType.tag, "barter"));
            raw.Tags.Add(new Tag(TagType.tag, "mulch"));
           

            IOfferMessage message= (OfferMessage)_target.Parse(raw);
            // actually this will certainly fail even for any kinda regex parser i can think of doing in a short time
            Assert.AreEqual(3, message.Tags.Count, "Expect count of tags to be 3 for message " + raw);
            Assert.AreEqual("#offr_test mulch available now in L:Paekakariki for #free #barter http://bit.ly/message0Info", message.OfferText, "Expect extracted message to work for " + raw);
        
        }

    }
}
