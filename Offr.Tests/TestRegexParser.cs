using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.Message;

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
        public void TestRegexMethod()
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
    }
}
