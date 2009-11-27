using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Offr.Location;
using Offr.Message;
using Offr.Repository;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestRegexParser
    {
        readonly IMessageParser _target;

        public TestRegexParser()
        {
            TagRepository tagRepository = new TagRepository();
            tagRepository.FilePath = "data/initial_tags.json";
            tagRepository.InitializeFromFile();
            //singletonTagProvider.Initialize();
            GoogleLocationProvider locationProvider = new MockLocationProvider();
            _target = new RegexMessageParser(tagRepository, locationProvider);
        }

        [Test]
        public void TestRegexMethodCleverTest()
        {
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                IOfferMessage messageGet = (OfferMessage) _target.Parse(rawMessage);
                foreach (ITag tag in rawMessage.Tags)
                {
                    Assert.That(messageGet.Tags.Contains(tag),  "Expected results to contain " + tag.match_tag + " for " + rawMessage);
                }
            }
        }

        [Test]
        public void TestRegexMethodSimpleTest()
        {

            MockRawMessage raw = new MockRawMessage(0)
                                     {
                                         Timestamp = DateTime.Now.AddHours(-2),
                                         CreatedBy = MockData.User0,
                                         Location = MockData.Location0,
                                         Thumbnail = "http://flickr.com/mulch.jpg",
                                         MoreInfoURL = "http://bit.ly/message0Info",
                                         Text = "#offr_test #ooooby mulch available now in l:Paekakariki: for #free http://bit.ly/message0Info pic here: http://flickr.com/mulch.jpg #mulch",
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
            Assert.AreEqual(raw.MoreInfoURL, message.MoreInfoURL);
            Assert.AreEqual(raw.Thumbnail, message.Thumbnail);
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
            Assert.AreEqual("#offr_test #ooooby mulch available now in l:Paekakariki: for #free http://bit.ly/message0Info pic here: http://flickr.com/mulch.jpg", message.OfferText,
                            "Expect extracted message to work for " + raw);
        }

        [Test]
        [Ignore("slows us down for now to hit live google, but good to have a test here - click to run expliclty")]
        public void TestRegexMethodLocationFailure()
        {
            MockRawMessage raw = new MockRawMessage(0)
            {
                Timestamp = DateTime.Now.AddHours(-5),
                CreatedBy = MockData.User0,
                Location = MockData.Location0,
                MoreInfoURL = "http://bit.ly/message0Info",
                Text = "#offr_test #ooooby mulch available now in l:Cardboard box in town: for #free http://bit.ly/message0Info #mulch",
                EndByText = null,
                EndBy = null
            };
            //List<ITag> expectedTags = new List<ITag>();
            IOfferMessage message = (OfferMessage) _target.Parse(raw);
            Assert.That(message.LocationTags.Count() == 0, "There should be no location tags");
        }

        [Test]
        public void TestGetMoreInfoUrl()
        {
            String link = "http://www.radsoftware.com.au/articles/regexsyntaxadvanced.gif";
            RegexMessageParser regexMessageParser = new RegexMessageParser(null,null);
            string s = regexMessageParser.TEST_GetMoreInfoUrl(link);
            Console.WriteLine();
        }
    }


}
    

