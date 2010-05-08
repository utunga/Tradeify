using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Offr.Json;
using Offr.Message;
using Offr.OpenSocial;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestOpenSocialMessageProviders
    {
        //this takes ages *yawn*
        [Ignore]
        [Test]
        public void TestOpenSocial()
        {
            Global.NotifyRawMessage(new OpenSocialRawMessage("oooby", "#ihave #vege in wellington", "Joav","blah", "http://blah"));

            IMessageRepository repository = Global.GetMessageRepository();
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
