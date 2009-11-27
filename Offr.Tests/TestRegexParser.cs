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
                    Assert.That(messageGet.Tags.Contains(tag),  "Expected results to contain " + tag.MatchTag + " for " + rawMessage);
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
                                         Thumbnail = "http://twitpic.com/show/thumb/r5aon",
                                         MoreInfoURL = "http://bit.ly/message0Info",
                                         Text = "#offr_test #ooooby mulch available now in L:Paekakariki: for #free http://bit.ly/message0Info pic here: http://twitpic.com/r5aon #mulch",
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
                Assert.That(message.Tags.Contains(tag), "Expected results to contain " + tag.MatchTag);
            }
            foreach (ITag tag in message.Tags)
            {
                Assert.That(expectedTags.Contains(tag), "Expected results to not contain" + tag.MatchTag);
            }
            
            Assert.AreEqual(raw.Text, message.OfferText,
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
            // a specific real example for which i know the query was failing
            string text = "SAVE $13 - A Bold Fresh Piece of Humanity $13.00 http://dealnay.com/8548 #book #nonfiction #offer";
            RegexMessageParser regexMessageParser = new RegexMessageParser(null,null);
            string moreInfoUrl = regexMessageParser.TEST_GetMoreInfoUrl(text);
            Assert.AreEqual("http://dealnay.com/8548", moreInfoUrl, "didn't received expected more info url");
        }

        [Test]
        public void TestGetImageUrl()
        {
            // a specific real example for which i know the query was failing
            string text = "#offr_test #ooooby mulch available now in L:Paekakariki: for #free http://bit.ly/message0Info pic here: http://flickr.com/mulch.jpg #mulch";
            RegexMessageParser regexMessageParser = new RegexMessageParser(null, null);
            string imageUrl = regexMessageParser.TEST_GetImageUrl(text);
            Assert.AreEqual("http://flickr.com/mulch.jpg", imageUrl, "didn't received expected image url");
        }

        [Test]
        public void TestGetImageUrlTwitpic()
        {
            // a specific real example for which i know the query was failing
            string text = "#offr_test #ooooby mulch available now in L:Paekakariki: for #free http://bit.ly/message0Info pic http://twitpic.com/r5aon #mulch";
            RegexMessageParser regexMessageParser = new RegexMessageParser(null, null);
            string imageUrl = regexMessageParser.TEST_GetImageUrl(text);
            Assert.AreEqual("http://twitpic.com/show/thumb/r5aon", imageUrl, "didn't received expected image url for twitpic");
        }

    }


}
    

