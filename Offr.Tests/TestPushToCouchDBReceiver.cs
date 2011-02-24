using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.CouchDB;
using Offr.Message;
using Offr.Repository;

namespace Offr.Tests
{
    [TestFixture]
    public class TestPushToCouchDBReceiver
    {
        IValidMessageReceiver _target;
        MockRawMessageProvider _mockProvider;
        MessageRepository _messageRepository;
        TagRepository _tagRepository;

        public TestPushToCouchDBReceiver()
        {
            _messageRepository = new MessageRepository();
            _tagRepository = new TagRepository();
            _target = new PushToCouchDBReceiver() { CouchServer = "http://xxx:yyy@chchneeds.couchone.com", CouchDB = "unit_test" };
        }

        [Test]
        public void TestPushMessage()
        {
            IncomingMessageProcessor processor = new IncomingMessageProcessor(_messageRepository, _tagRepository,  new MockMessageParser(), _target);
            _mockProvider = new MockRawMessageProvider(processor);
            _mockProvider.Update();
        }
    }
}
