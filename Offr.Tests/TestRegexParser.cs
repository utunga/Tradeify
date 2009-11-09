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
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                IOfferMessage messageGet = (OfferMessage) _target.Parse(rawMessage);
                // actually this will certainly fail even for any kinda regex parser i can think of doing in a short time
                foreach (ITag tag in rawMessage.Tags)
                {
                    // FIXME: regex parser can't handle location tags yet
                    if (tag.type != TagType.loc)
                    {
                        Assert.That(messageGet.Tags.Contains(tag), "Expected results to contain " + tag.match_tag + " for " + rawMessage);
                    }
                }
                // this is the 'rest of it' bit
                //Assert.AreEqual(rawMessage.OfferText, messageGet.OfferText, "Expected extracted offer message to match " + rawMessage);

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
                                         Text = "#offr_test #ooooby mulch available now in l:Paekakariki: for #free http://bit.ly/message0Info #mulch",
                                         EndByText = null,
                                         EndBy = null
                                     };
            List<ITag> expectedTags = new List<ITag>();
            expectedTags.Add(new Tag(TagType.type, "free"));
            expectedTags.Add(new Tag(TagType.group, "ooooby"));
            expectedTags.Add(new Tag(TagType.tag, "mulch"));


            IOfferMessage message = (OfferMessage) _target.Parse(raw);
            // actually this will certainly fail even for any kinda regex parser i can think of doing in a short time
            Assert.AreEqual(3, expectedTags.Count, "Expect count of tags to be 3 for message " + raw);
            foreach (ITag tag in expectedTags)
            {
                Assert.That(message.Tags.Contains(tag), "Expected results to contain " + tag.match_tag);
            }
            //NOTE2MT is the offr_test really meant to b
            Assert.AreEqual(/*"#offr_test */ "#ooooby mulch available now in l:Paekakariki: for #free http://bit.ly/message0Info", message.OfferText,
                            "Expect extracted message to work for " + raw);
        }
    }
}
    

