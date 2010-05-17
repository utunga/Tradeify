using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Offr.Location;
using Offr.Message;
using Offr.Repository;
using Offr.RSS;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestRSSProvider : IRawMessageReceiver
    {
        List<IRawMessage> _receivedMessages = new List<IRawMessage>();

        public void Notify(IEnumerable<IRawMessage> messages)
        {
            _receivedMessages.AddRange(messages);
        }

        public void Notify(IRawMessage message)
        {
            _receivedMessages.Add(message);
        }

        [Test]
        public void TestGetRawMessages()
        {
            RSSRawMessageRepository seenUpdates = new RSSRawMessageRepository();
            RSSRawMessageProvider rssProvider = new RSSRawMessageProvider("ooooby", "http://localhost:60600/data/activity_test0.rss", this, seenUpdates, new MockWebRequestFactory());
            rssProvider.Update();

            // somewhat of an integration test, but gets us some of the way there
            foreach (IRawMessage message in _receivedMessages)
            {

                 Console.Out.Write(message.CreatedBy + " | ");
                 Console.Out.Write(message.Timestamp + " | ");
                 Console.Out.Write(message.Text + " | ");
                 Console.Out.WriteLine();
            }

            Assert.AreEqual(10, _receivedMessages.Count, "Expect 10 messages because 10 messages, including some non valid ones, are in the tests data");

            //reset recieved messages
            _receivedMessages = new List<IRawMessage>();
            rssProvider = new RSSRawMessageProvider("ooooby", "http://localhost:60600/data/activity_test1.rss", this, seenUpdates, new MockWebRequestFactory());
            rssProvider.Update();
            Assert.AreEqual(0, _receivedMessages.Count, "Expect no messages on this update, because there are no new updates");

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
