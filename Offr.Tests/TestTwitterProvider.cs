using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Offr.Location;
using Offr.Message;
using Offr.Text;
using Ninject.Core;
using Ninject;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestTwitterProvider
    {
        IMessageProvider _target;

        public TestTwitterProvider()
        {
            //for this test create real objects all the way down the line - so...more of an integration test really
            // (which is why this is disabled)
            TwitterRawMessageProvider twitterProvider = new TwitterRawMessageProvider(MessageType.offr_test);
            TagProvider singletonTagProvider = new TagProvider();
            GoogleLocationProvider locationProvider = new GoogleLocationProvider();
            RegexMessageParser realMessageParser = new RegexMessageParser(singletonTagProvider, locationProvider);
            _target = new MessageProvider(twitterProvider, realMessageParser);
        }

        [Test]
        [Ignore("Cannot reliably test against live twitter feed so disable for now")]
        public void TestGetMessages()
        {
            // somewhat of an integration test, but gets us some of the way there
            List<IMessage> output = new List<IMessage>( _target.AllMessages);
            foreach (IMessage message in output)
            {
                 Console.Out.Write(message.MessageType.ToString() + " | ");
                 Console.Out.Write(message.TimeStamp.ToString() + " | ");
                 Console.Out.Write(message.Source.ToString() + " | ");
                 Console.Out.WriteLine();
            }

            Assert.AreEqual(1, output.Count);
        }

        [Test]
        [Ignore("Cannot reliably test against live twitter feed so disable for now")]
        public void TestMessageByID()
        {
            // somewhat of an integration test, actually
            IMessage msg = _target.MessageByID("1810947076");
            Assert.IsNotNull(msg);
            Assert.AreEqual("1810947076", msg.Source.Pointer.ProviderMessageID);

            msg = _target.MessageByID("123223");
            Assert.IsNull(msg);
        }

    }
}
