using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Offr.Json;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestOpenSocialMessageProviders
    {
        [Test]
        public void TestParse()
        {
            OpenSocialMessageProvider provider = Global.Kernel.Get<OpenSocialMessageProvider>();
            IMessageRepository repository= Global.Kernel.Get<IMessageRepository>();
            provider.ParseMessage("#ihave #vege in wellington","Joav","blah");
            foreach (IMessage message in repository.AllMessages())
            {
                if (message.RawText.Equals("#ihave #vege in wellington"))
                {
                    string s=JSON.Serialize(message);
                    JSON.Deserialize<IMessage>(s);
                    return;
                }
               
            }        
            Assert.That(false);
        }
    }
}
