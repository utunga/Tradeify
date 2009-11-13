using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageListSerializer
    {

        public TestMessageListSerializer()
        {
            Global.Initialize(new TestModule());
        }

        [Test]
        public void TestSerializeAllMessages()
        {

            IMessageProvider provider = Global.Kernel.Get<IMessageProvider>();
            IEnumerable<IMessage> messages = provider.AllMessages;
     
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Register the custom converter.
            serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });

            List<IMessage> messagesToSend = new List<IMessage>(messages.Take(10));
            //Console.Out.Write(serializer.Serialize(messagesToSend));
            Console.Out.Write(JsonConvert.SerializeObject(messagesToSend));
        }
    }
}