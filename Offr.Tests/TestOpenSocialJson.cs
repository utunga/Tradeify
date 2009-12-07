using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;
using NUnit.Framework;
using Offr.Json;
using Offr.Open_Social;
namespace Offr.Tests
{
    [TestFixture]
    public class TestOpenSocialJson
    {
        private string json = @"{""Message"":""b;a"",""User"":{""fields_"":{""photos"":[{""linkText"":""Joav"",""primary"":true,""value"":""http://api.ning.com:80/files/ojJR0x7XjgKkCg6JW0BbUO1R3DYiSYEwoD49ysquHWI_/455779645.png?crop=1%3A1"",""type"":""thumbnail""}],""id"":""0asph7yumi8p0"",""ning.admin"":true,""ning.creator"":true,""profileUrl"":""http://tradeify.ning.com/profile/Joav"",""isViewer"":true,""urls"":[{""fields_"":{""linkText"":""View Joav's page on tradeify"",""primary"":true,""value"":""http://tradeify.ning.com/profile/Joav"",""type"":""profile"",""address"":""http://tradeify.ning.com/profile/Joav""}}],""thumbnailUrl"":""http://api.ning.com:80/files/ojJR0x7XjgKkCg6JW0BbUO1R3DYiSYEwoD49ysquHWI_/455779645.png?crop=1%3A1"",""name"":{""fields_"":{""formatted"":""Joav"",""unstructured"":""Joav""}},""isOwner"":true,""displayName"":""Joav""},""isOwner_"":true,""isViewer_""a:true}}
";
        [Test]
        public void testParse()
        {
           /* OpenSocialJson jsonObject=JSON.Deserialize<OpenSocialJson>(json);
            Assert.AreEqual("b;a", jsonObject.Message);
            Assert.AreEqual("Joav", jsonObject.User.fields_.displayName);
            Assert.AreEqual("http://api.ning.com:80/files/ojJR0x7XjgKkCg6JW0BbUO1R3DYiSYEwoD49ysquHWI_/455779645.png?crop=1%3A1", jsonObject.User.fields_.displayName);*/
        }
    }
}
