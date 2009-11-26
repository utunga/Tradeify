using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Offr.Message;
using Offr.Repository;
using Offr.Services;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestPersistanceService
    {
        private MessageRepository _messageRepository;
        private string _filePath;
        private BackgroundExceptionReceiver _receiver;

        [TestFixtureSetUp]
        public void SetUp()
        {
            // if the IMessageRepository being provided by kernal is *not* a MessageRepository none of this test is going to work
            _messageRepository = (MessageRepository) Global.Kernel.Get<IMessageRepository>();
            _receiver = new BackgroundExceptionReceiver();
            _filePath = "data/empty_offer.json";
            _messageRepository.FilePath = _filePath;
            Assert.That(_messageRepository.AllMessages().Count() == 0,"File should be initially empty");
        }
        [Test]
        public void TestStartup()
        {
            Assert.That(!PersistanceService.IsBusy);
            PersistanceService.EnsureStarted(_receiver);
            Thread.Sleep(50);
            Assert.That(PersistanceService.IsBusy);
        }
        [Test]
        public void TestSavePersistance()
        {
            OfferMessage offerMessage = new OfferMessage();
            offerMessage.MessagePointer = new TwitterMessagePointer(100);
            _messageRepository.Save(offerMessage);
                        
            MessageRepository updated = new MessageRepository();
            updated.FilePath = _filePath;
            
            PersistanceService.Stop();
            while (PersistanceService.IsBusy);

            updated.InitializeFromFile();
            Assert.That(updated.AllMessages().Count() == 1);
        }
        [Test]
        public void TestStop()
        {
            PersistanceService.EnsureStarted(_receiver);
            Assert.That(PersistanceService.IsBusy);
            PersistanceService.Stop();
            Thread.Sleep(50);
            Assert.That(!PersistanceService.IsBusy);
        }
        [TestFixtureTearDown]
        public void TearDown()
        {
            PersistanceService.Stop();
            TextWriter tw = new StreamWriter(_filePath);
            tw.Write("");
            tw.Close();   
        }
    }
}
