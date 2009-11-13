using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Offr.Message;
using Offr.Text;
using Ninject.Core;
using Ninject;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageProvider
    {
        IMessageProvider _target;

        public TestMessageProvider()
        {
            Global.Initialize(new TestModule());
            _target = Global.Kernel.Get<IMessageProvider>(); 
        }

        [Test]
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

            Assert.AreEqual(MockData.MSG_COUNT, output.Count);
        }

        [Test]
        public void TestMessageByID()
        {
            // somewhat of an integration test, actually
            IMessage msg = _target.MessageByID("0");
            Assert.IsNotNull(msg);
            Assert.AreEqual("0", msg.Source.Pointer.ProviderMessageID);

            msg = _target.MessageByID("1");
            Assert.IsNotNull(msg);
            Assert.AreEqual("1", msg.Source.Pointer.ProviderMessageID);
        }

    }
}
