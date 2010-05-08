using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.Common;
using Offr.Demo;
using Offr.Location;
using Offr.Message;
using Offr.Repository;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestDemoData
    {
        RegexMessageParser _parser;

        [TestFixtureSetUp]
        public void Setup()
        {
            TagRepository tagRepository = new TagRepository();
            tagRepository.FilePath = "data/initial_tags.json";
            tagRepository.InitializeFromFile();
            ILocationProvider locationProvider = new MockLocationProvider();
            _parser = new RegexMessageParser(tagRepository, locationProvider);
        }

        [Test]
        public void TestParseAllDemoData()
        {
            foreach (IRawMessage rawMessage in DemoData.RawMessages)
            {
                IMessage message = _parser.Parse(rawMessage);
                Assert.That(message.IsValid(), "Demo message " + message + " was not valid." + Util.ConcatStringArray(message.ValidationFailReasons()));
            }
        }

        [Test]
        public void TestParseAllMockData()
        {
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                IMessage message = _parser.Parse(rawMessage);
                Assert.That(message.IsValid(), "Mock message " + message + " was not valid." + Util.ConcatStringArray(message.ValidationFailReasons()));
            }
        }
    }
}
