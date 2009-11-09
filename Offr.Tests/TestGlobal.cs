using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NUnit.Framework;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestGlobal
    {

        public TestGlobal()
        {
            Global.Initialize(new TestModule());
        }

        [Test]
        public void InitializeWithRecentOffers_BlowsUpWithWrongFile()
        {
            try
            {
                Global.InitializeWithRecentOffers("data/typo_asdfuihsg.json"); // should be copied to bin/Debug output directory because of build action properties on that file 
                Assert.Fail("Expected to get an  exception from trying to trying to load bad file");
            }
            catch (IOException)
            {
                //expected
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected to get an IOexception from trying to trying to load bad file, instead got:" + ex);
            }
        }

        [Test]
        public void TestInitializeWithRecentOffers_Works()
        {
            Global.InitializeWithRecentOffers("data/initial_offers.json"); // should be copied to bin/Debug output directory because of build action properties on that file 
            Assert.AreEqual(10, Global.Kernel.Get<IMessageProvider>().AllMessages.Count, "Expected to load 10 messages");
            
        }
    }
}