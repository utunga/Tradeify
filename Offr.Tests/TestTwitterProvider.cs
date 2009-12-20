using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Offr.Location;
using Offr.Message;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestTwitterProvider
    {
        IMessageProvider _target;

        public void InitializeTwitterProvider()
        {
            //for this test create real objects all the way down the line - so...more of an integration test really
            // (which is why this is disabled)
            TwitterRawMessageProvider twitterProvider = new TwitterRawMessageProvider(MessageType.offr);
            TagRepository singletonTagProvider = new TagRepository();
            singletonTagProvider.FilePath = "data/initial_tags.json";
            singletonTagProvider.InitializeFromFile();
            GoogleLocationProvider locationProvider = new GoogleLocationProvider();
            RegexMessageParser realMessageParser = new RegexMessageParser(singletonTagProvider, locationProvider);
            MessageRepository messageRepository = new MessageRepository();
            _target = new MessageProvider(messageRepository, twitterProvider, realMessageParser);
        }

        [Test]
        [Ignore("Cannot reliably test against live twitter feed so disable for now")]
        public void TestGetMessages()
        {
            InitializeTwitterProvider();
            // somewhat of an integration test, but gets us some of the way there
            List<IMessage> output = new List<IMessage>( _target.AllMessages);
            foreach (IMessage message in output)
            {
                 Console.Out.Write(message.MessageType.ToString() + " | ");
                 Console.Out.Write(message.Timestamp.ToString() + " | ");
                 //Console.Out.Write(message.Source.ToString() + " | ");
                 Console.Out.WriteLine();
            }

            Assert.AreEqual(1, output.Count);
        }

        //[Test]
        //[Ignore("Cannot reliably test against live twitter feed so disable for now")]
        //public void TestMessageByID()
        //{
        //    // somewhat of an integration test, actually
        //    IMessage msg = _target.MessageByID("1810947076");
        //    Assert.IsNotNull(msg);
        //    Assert.AreEqual("1810947076", msg.MessagePointer.ProviderMessageID);

        //    msg = _target.MessageByID("123223");
        //    Assert.IsNull(msg);
        //}

    }
}
