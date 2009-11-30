using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestTwitterPollingService
    {
        private static TwitterRawMessageProvider twitterRawMessageProvider = new TwitterRawMessageProvider();
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
            twitterRawMessageProvider.Update();
        }
    }
}
