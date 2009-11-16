using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NUnit.Framework;
using Offr.Twitter;

namespace Offr.Tests
{
    [TestFixture]
    public class TestJSONTwitterParser
    {
        private const string TwitterVerifyJSON =
            @"{""results"":[{""text"":""#offr_test test offer message"",""to_user_id"":null,""from_user"":""twollar_test_1"",""id"":1810947076,""from_user_id"":9350275,""iso_language_code"":""da"",""source"":""&lt;a href=&quot;http:\/\/twitter.com\/&quot;&gt;web&lt;\/a&gt;"",""profile_image_url"":""http:\/\/static.twitter.com\/images\/default_profile_normal.png"",""created_at"":""Fri, 15 May 2009 22:32:03 +0000""}],""since_id"":0,""max_id"":1812602634,""refresh_url"":""?since_id=1812602634&q=%23offr_test"",""results_per_page"":15,""total"":1,""completed_in"":0.0841820000000001,""page"":1,""query"":""%23offr_test""}";

        //[Test]
        //public void TestResultsParse()
        //{
        //    TwitterResultSet resultSet = JSONTwitterParser.ParseSearchResults(TwitterVerifyJSON);
        //    Assert.AreEqual(1, resultSet.results.Count());
        //    Assert.AreEqual("twollar_test_1", resultSet.results[0].from_user);
        //    Assert.AreEqual(null, resultSet.results[0].to_user_id);
        //}

        [Test]
        public void TestResultsDeSerialize()
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TwitterResultSet resultSet = serializer.Deserialize<TwitterResultSet>(TwitterVerifyJSON);
            Assert.AreEqual(1, resultSet.results.Count());
            Assert.AreEqual("twollar_test_1", resultSet.results[0].from_user);
            Assert.AreEqual(null, resultSet.results[0].to_user_id);
        }


        [Test]
        public void TestUserSerialize()
        {
            //User user = new User();
            //user.ScreenName = "foo";

            //StringBuilder xml = new StringBuilder();
            //StringWriter writer = new StringWriter(xml);
            //new System.Xml.Serialization.XmlSerializer(typeof(User)).Serialize(writer, user);
            //Console.Out.Write(xml.ToString());
        }

    }
}
