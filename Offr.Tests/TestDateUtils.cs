using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestDateUtils
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            //DateTime.Parse("23 June 2009 23:23:23") assumes the string is provided in GMT 
            DateUtils.TestingNow = DateTime.Parse("23 June 2009 23:23:23"); 
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            DateUtils.TestingNow = null;
        }

        [Test]
        public void TEST_FriendlyLocalTimeStampFromUTC()
        {
            Assert.AreEqual("Today, 11:10 PM",
                DateUtils.FriendlyLocalTimeStampFromUTC(DateTime.Parse("23 June 2009 23:10:01").ToUniversalTime()), 
                "Time from 'today' formatted wrong");

            Assert.AreEqual("20 Jun, 12:10 AM",
                DateUtils.FriendlyLocalTimeStampFromUTC(DateTime.Parse("20 June 2009 0:10:01").ToUniversalTime()),
                "Time from this week, formatted wrong");

            Assert.AreEqual("20 Jun, 1:10 PM",
               DateUtils.FriendlyLocalTimeStampFromUTC(DateTime.Parse("20 June 2009 13:10:57").ToUniversalTime()),
               "Time from this week, formatted wrong");
            
            Assert.AreEqual("10 May 2009",
                DateUtils.FriendlyLocalTimeStampFromUTC(DateTime.Parse("10 May 2009 23:10:01").ToUniversalTime()),
                "Date from a few months previous formatted wrong");

            Assert.AreEqual("23 Jun 2008",
                DateUtils.FriendlyLocalTimeStampFromUTC(DateTime.Parse("23 June 2008 23:10:01").ToUniversalTime()),
                "Date from previous year formatted wrong");
        }

        [Test]
        public void TEST_UTCDateTimeFromTwitterTimeStamp()
        {
            DateTime actual = DateUtils.UTCDateTimeFromTwitterTimeStamp("Fri, 06 Nov 2009 23:34:48 +0000");
            DateTime expected = new DateTime(2009,11,6,23,34,48,DateTimeKind.Utc);
            Assert.AreEqual(expected, actual, "Failed to parse date in twitter format (looks like)");
        }
    }
}
