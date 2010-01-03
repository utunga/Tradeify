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
            CheckUTC("Today, 11:10 PM", "23 June 2009 23:10:01");
            CheckUTC("20 Jun, 12:10 AM", "20 June 2009 0:10:01");
            CheckUTC("20 Jun, 1:10 PM", "20 June 2009 13:10:57");
            CheckUTC("10 May 2009", "10 May 2009 23:10:01");
            CheckUTC("23 Jun 2008", "23 June 2008 23:10:01");
        }


        //Curse you DATETIME, come up with a real fix one day.. maybe
        private void CheckUTC(string expected, string timeToParse)
        {
            string replaced = expected.Replace("PM", "p.m.");
            replaced = replaced.Replace("AM", "a.m.");
            string utc = DateUtils.FriendlyLocalTimeStampFromUTC(DateTime.Parse(timeToParse).ToUniversalTime());
            bool usTimeFormat = Equals(expected,utc);
            bool nzTimeFormat = Equals(replaced, utc);
            Assert.That(usTimeFormat || nzTimeFormat, "Time for 'today' formatted wrong");           
        }

        [Test]
        public void TEST_UTCDateTimeFromTwitterTimeStamp()
        {
            DateTime actual = DateUtils.UTCDateTimeFromTwitterTimeStamp("Fri, 06 Nov 2009 23:34:48 +0000");
            DateTime expected = new DateTime(2009,11,6,23,34,48,DateTimeKind.Utc);
            Assert.AreEqual(expected, actual, "Failed to parse date in twitter format (looks like)");
        }

        [Test]
        public void TEST_TwitterTimeStampFromUTCRoundTrip()
        {
            DateTime expected = new DateTime(2009, 11, 6, 23, 34, 48, DateTimeKind.Utc);
            
            DateTime start = DateUtils.UTCDateTimeFromTwitterTimeStamp("Fri, 06 Nov 2009 23:34:48 GMT");
            string timestamp = DateUtils.TwitterTimeStampFromUTCDateTime(start);
            DateTime roundTrip = DateUtils.UTCDateTimeFromTwitterTimeStamp(timestamp);
            
            Assert.AreEqual(expected, roundTrip, "Failed to roundtrip date through twitter timestamp");
            Assert.AreEqual("Fri, 06 Nov 2009 23:34:48 GMT", timestamp, "Failed to export twitter timestamp");

        }
    }
}
