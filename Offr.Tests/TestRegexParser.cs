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
                    Assert.That(messageGet.Tags.Contains(tag),
                                "Expected results to contain " + tag.match_tag + " for " + rawMessage);
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
            expectedTags.Add(new Tag(TagType.loc, "Paekakariki"));
            expectedTags.Add(new Tag(TagType.loc, "New Zealand"));
            expectedTags.Add(new Tag(TagType.loc, "NZ"));

            IOfferMessage message = (OfferMessage) _target.Parse(raw);
            // actually this will certainly fail even for any kinda regex parser i can think of doing in a short time
            Assert.AreEqual(6, expectedTags.Count, "Expect count of tags to be 6 for message " + raw);
            foreach (ITag tag in expectedTags)
            {
                Assert.That(message.Tags.Contains(tag), "Expected results to contain " + tag.match_tag);
            }
            foreach (ITag tag in message.Tags)
            {
                Assert.That(expectedTags.Contains(tag), "Expected results to not contain" + tag.match_tag);
            }
            //NOTE2MT is the offr_test really meant to b
            Assert.AreEqual("#offr_test #ooooby mulch available now in l:Paekakariki: for #free http://bit.ly/message0Info", message.OfferText,
                            "Expect extracted message to work for " + raw);
        }
        [Test]
        public void TestRegexMethodLocationFailure()
        {
            MockRawMessage raw = new MockRawMessage(0)
            {
                Timestamp = "10pm",
                CreatedBy = MockData.User0,
                Location = MockData.Location0,
                MoreInfoURL = "http://bit.ly/message0Info",
                Text = "#offr_test #ooooby mulch available now in l:Cardboard box in town: for #free http://bit.ly/message0Info #mulch",
                EndByText = null,
                EndBy = null
            };
            //List<ITag> expectedTags = new List<ITag>();
            IOfferMessage message = (OfferMessage) _target.Parse(raw);
            Assert.That(message.LocationTags.Count == 0, "There should be no location tags");
        }
    }

}
    

