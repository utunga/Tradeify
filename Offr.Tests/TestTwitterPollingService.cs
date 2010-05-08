using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestTwitterPollingService : IRawMessageReceiver
    {
        private static TwitterRawMessageProvider _target;

        [SetUp]
        public void SetUp()
        {
            _target = new TwitterRawMessageProvider(this, new MockWebRequestFactory());
        }

        [Test]
        public void TestSimultaneousUpdate()
        {
            for (int i = 0; i < 1000; i++)
            {
                Thread t = new Thread(Run);
                t.Start();
            }
        }

        static void Run()
        {
            int next = new Random().Next();
            Thread.Sleep(next);
            _target.Update();
        }

        #region Implementation of IRawMessageReceiver

        public void Notify(IEnumerable<IRawMessage> messages)
        {
            //don't need to do anything
        }

        public void Notify(IRawMessage messages)
        {
            //don't need to do anything
        }

        #endregion
    }
}
