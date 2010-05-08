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
            RSSRawMessageProvider rssProvider = new RSSRawMessageProvider("ooooby", "http://localhost:60600/data/activity.rss", this, new MockWebRequestFactory());
            rssProvider.Update();

            // somewhat of an integration test, but gets us some of the way there
            foreach (IRawMessage message in _receivedMessages)
            {

                 Console.Out.Write(message.CreatedBy + " | ");
                 Console.Out.Write(message.Timestamp + " | ");
                 Console.Out.Write(message.Text + " | ");
                 Console.Out.WriteLine();
            }

            Assert.AreEqual(10, _receivedMessages.Count);
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
