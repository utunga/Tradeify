using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Offr.Twitter;
using Offr.Users;

namespace Offr.Tests
{
    [TestFixture]
    public class TestXMLTwitterParser
    {
        private const string TwitterVerifyXML =
            @"<?xml version=""1.0"" encoding=""UTF-""?>
                <user>
                  <id>5427252</id>
                  <name>Miles</name>
                  <screen_name>utunga</screen_name>
                  <location></location>
                  <description>Sentient Being</description>
                  <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/82440779/miles_normal.jpg</profile_image_url>
                  <url>http://milo.graytime.org</url>
                  <protected>false</protected>
                  <followers_count>57</followers_count>
                  <profile_background_color>000000</profile_background_color>
                  <profile_text_color>000000</profile_text_color>
                  <profile_link_color>0000ff</profile_link_color>
                  <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
                  <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
                  <friends_count>59</friends_count>
                  <created_at>Mon Apr 23 09:28:59 +0000 2007</created_at>
                  <favourites_count>10</favourites_count>
                  <utc_offset>43200</utc_offset>
                  <time_zone>Wellington</time_zone>
                  <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/2660643/100974127_89a57c2b5c.jpg</profile_background_image_url>
                  <profile_background_tile>true</profile_background_tile>
                  <statuses_count>239</statuses_count>
                  <notifications>false</notifications>
                  <following>false</following>
                  <status>
                    <created_at>Sat May 16 00:40:51 +0000 2009</created_at>
                    <id>1811975616</id>
                    <text>Hello @swhitley - Testing the .NET oAuth API</text>
                    <source>&lt;a href=""http://www.twademe.org""&gt;TwadeMe&lt;/a&gt;</source>
                    <truncated>false</truncated>
                    <in_reply_to_status_id></in_reply_to_status_id>
                    <in_reply_to_user_id></in_reply_to_user_id>
                    <favorited>false</favorited>
                    <in_reply_to_screen_name></in_reply_to_screen_name>
                  </status>
                </user>";
        [Test]
        public void TestUserParse()
        {
            User user = XmlTwitterParser.ParseUser(TwitterVerifyXML);
            Assert.That(user.ScreenName == "utunga");
        }

        [Test]
        public void TestUserDeSerialize()
        {

            StringReader reader = new StringReader(TwitterVerifyXML);
            XmlSerializer serializer = new XmlSerializer(typeof(User));

            User user = serializer.Deserialize(reader) as User;
            Assert.That(user.ScreenName == "utunga");
       
        }


        [Test]
        public void TestUserSerialize()
        {
            User user = new User();
            user.ScreenName = "foo";

            StringBuilder xml = new StringBuilder();
            StringWriter writer = new StringWriter(xml);
            new System.Xml.Serialization.XmlSerializer(typeof(User)).Serialize(writer, user);
            Console.Out.Write(xml.ToString());
        }

    }
}
