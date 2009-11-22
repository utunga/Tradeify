using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageRepository
    {
        private MessageRepository _target;

        public TestMessageRepository()
        {
        }

        [Test]
        public void InitializeWithRecentOffers_BlowsUpWithWrongFile()
        {
            try
            {
                //Global.InitializeWithRecentOffers("data/typo_asdfuihsg.json"); // should be copied to bin/Debug output directory because of build action properties on that file 
                _target = new MessageRepository();
                _target.InitializeFromFile("data/typo_asdfuihsg.json");
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
          public void TestInitialize()
          {
              string jsonFilePath = "data/initial_offers.json";
              _target = new MessageRepository();
              _target.InitializeFromFile(jsonFilePath);

              Console.Out.WriteLine("Initialized from file with following data");
              Console.Out.WriteLine(JSON.Serialize(_target.GetAll()));

              List<IMessage> messages = new List<IMessage>(_target.GetAll());

              Assert.AreEqual(6, messages.Count, "Expected 6 messages after initializing from " + jsonFilePath);
          }


    }
   
}