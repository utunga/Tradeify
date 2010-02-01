using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Offr.Message;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageProvider
    {
        IRawMessageReceiver _target;
        MockRawMessageProvider _mockProvider;
        MessageRepository _messageRepository;
        TagRepository _tagRepository;

        public TestMessageProvider()
        {
            MockMessageParser mockParser = new MockMessageParser();
            _messageRepository = new MessageRepository();
            _tagRepository = new TagRepository();
            IncomingMessageProcessor target = new IncomingMessageProcessor(_messageRepository, _tagRepository, mockParser);
            _mockProvider = new MockRawMessageProvider(target);
            _mockProvider.Update();
            _target = target;

        }

        [Test]
        public void TestGetMessages()
        {
            // somewhat of an integration test, but gets us some of the way there
           
            List<IMessage> output = new List<IMessage>( _messageRepository.AllMessages());
            foreach (IMessage message in output)
            {
                 Console.Out.Write(message.MessageType.ToString() + " | ");
                 Console.Out.Write(message.Timestamp + " | ");
                 //Console.Out.Write(message.Source.ToString() + " | ");
                 Console.Out.WriteLine();
            }

            Assert.AreEqual(MockData.MSG_COUNT, output.Count);
        }

        //[Test]
        //public void TestMessageByID()
        //{
        //    // somewhat of an integration test, actually
        //    IMessage msg = _target.MessageByID("twitter/0");
        //    Assert.IsNotNull(msg);
        //    Assert.AreEqual("0", msg.MessagePointer.ProviderMessageID);

        //    msg = _target.MessageByID("twitter/1");
        //    Assert.IsNotNull(msg);
        //    Assert.AreEqual("1", msg.MessagePointer.ProviderMessageID);
        //}

    }
}
