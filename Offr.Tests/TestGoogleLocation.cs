using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Offr.Location;
using System.IO;
using Offr.Text;
using NUnit.Framework;

namespace Offr.Tests
{
    [TestFixture]
    public class TestGoogleLocation
    {

        #region "1600+Amphitheatre+Parkway,+Mountain+View,+CA&output=json&oe=utf8"
        //private string search =
          // "1600+Amphitheatre+Parkway,+Mountain+View,+CA&output=json&oe=utf8";
        const string _testJSON = @"{
  ""name"": ""1500 Amphitheatre Parkway, Mountain View, CA"",
  ""Status"": {
    ""code"": 200,
    ""request"": ""geocode""
  },
  ""Placemark"": [ {
    ""id"": ""p1"",
    ""address"": ""1500 Amphitheatre Pkwy, Mountain View, CA 94043, USA"",
    ""AddressDetails"": {
   ""Accuracy"" : 8,
   ""Country"" : {
      ""AdministrativeArea"" : {
         ""AdministrativeAreaName"" : ""CA"",
         ""SubAdministrativeArea"" : {
            ""Locality"" : {
               ""LocalityName"" : ""Mountain View"",
               ""PostalCode"" : {
                  ""PostalCodeNumber"" : ""94043""
               },
               ""Thoroughfare"" : {
                  ""ThoroughfareName"" : ""1500 Amphitheatre Pkwy""
               }
            },
            ""SubAdministrativeAreaName"" : ""Santa Clara""
         }
      },
      ""CountryName"" : ""USA"",
      ""CountryNameCode"" : ""US""
   }
},
    ""ExtendedData"": {
      ""LatLonBox"": {
        ""north"": 37.4264181,
        ""south"": 37.4201229,
        ""east"": -122.0773318,
        ""west"": -122.0836270
      }
    },
    ""Point"": {
      ""coordinates"": [ -122.0804822, 37.4232890, 0 ]
    }
  } ]
}";

        #endregion
        
        private SortedList<string, ILocation> _addressToExpectedTags;

        [TestFixtureSetUp]
        public void Setup()
        {
            _addressToExpectedTags = new SortedList<string, ILocation>();
            //_addressToExpectedTags.Add("30 Fitzroy Street, New Plymouth, NZ", new[] { "nz", "taranaki", "fitzroy"});
            string address = "1600+Amphitheatre+Parkway,+Mountain+View,+CA&output=json&oe=utf8";
            List<ITag> tags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "Mountain View")),
                                      (new Tag(TagType.loc, "CA")),
                                      (new Tag(TagType.loc, "USA")),
                                      (new Tag(TagType.loc, "US"))
                                  };
            Console.WriteLine("tag count"+tags.Count);
            Location.Location google = new Location.Location
                                           {
                                               GeoLat = (decimal) 37.4217590,
                                               GeoLong = (decimal) -122.0843700,
                                               SourceText = address,
                                               _locationTags = tags

                                           };
            google._locationTags = tags;
            _addressToExpectedTags.Add(address, google);
            address = "30+Fitzroy+Street,+New+Plymouth,+NZ&output=json&oe=utf";
            tags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "New Plymouth")),
                                      (new Tag(TagType.loc, "Taranaki")),
                                      (new Tag(TagType.loc, "New Zealand")),
                                      (new Tag(TagType.loc, "NZ"))
                                  };
            Location.Location fitzroy = new Location.Location
            {
                GeoLat = (decimal)-39.0443597,
                GeoLong = (decimal)174.1080569,
                SourceText = address,
                _locationTags = tags

            };
           fitzroy._locationTags = tags;
            //_addressToExpectedTags.Add(address, fitzroy);

           address = "20+Lambton+Quay,+Wellington,+Wellington&output=json&oe=utf8";
           tags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "Wellington")),
                                      (new Tag(TagType.loc, "Wellington")),
                                      (new Tag(TagType.loc, "New Zealand")),
                                      (new Tag(TagType.loc, "NZ"))
                                  };
           Location.Location lambton = new Location.Location
           {
               GeoLat = (decimal)-41.2787026,
               GeoLong = (decimal)174.7785408,
               SourceText = address,
               _locationTags = tags

           };
           lambton._locationTags = tags;
           _addressToExpectedTags.Add(address, lambton);
        }

        [Test]
        public void TestCorrectTagsAreRetrieved()
        {

        }

        [Test]
        public void TestDeserialize()
        {
            //StreamReader streamReader = new StreamReader("C:/Users/joav/Desktop/offr/Offr.Tests/GoogleLocationTest.txt");
            //string text = streamReader.ReadToEnd();
            //Console.WriteLine(text);
            //streamReader.Close();
           //Console.WriteLine(_testJSON);
            GoogleResultSet resultSet = (new JavaScriptSerializer()).Deserialize<GoogleResultSet>(_testJSON);
            Assert.AreEqual("1500 Amphitheatre Parkway, Mountain View, CA", resultSet.name, "name did not serialize correctly");
            
            
           
        }
        [Test]
        public void TestParse()
        {
            GoogleLocationProvider g=new GoogleLocationProvider();
            foreach(string search in _addressToExpectedTags.Keys) {
                ILocation location = g.Parse(search);
                ILocation expected = _addressToExpectedTags[search];
                List<ITag> list = location._locationTags;
                List<ITag> tags = expected._locationTags;
                //Console.WriteLine("count " + location._locationTags.Count);
                Assert.IsTrue(location.Equals(expected), "Location not as expected");
}
            
         // ILocation  location = g.Parse(search);
           // Assert.AreEqual("USA", location.Country,"Country is not as expected");
          //Assert.AreEqual(search, location.SourceText, "Country is not as expected");
          //Assert.AreEqual(search, location.SourceText, "Country is not as expected");

        }

        //[TestMethod]
        //public void TestStatus()
        //{
        //    GoogleLocationProvider g = new GoogleLocationProvider();
        //    string name = g.getStatus();
        //    Console.WriteLine(name);
        //    Assert.IsTrue(false);
        //    //Assert.IsTrue(g.getIlocation().Equals("1500 Amphitheatre Parkway, Mountain View, CA"));
        //}

        //[TestMethod]
        //public void TestName()
        //{
        //    GoogleLocationProvider g = new GoogleLocationProvider();
        //    string name=g.getIlocation();
        //    Console.WriteLine(name);
        //    Assert.IsTrue(g.getIlocation().Equals("1500 Amphitheatre Parkway, Mountain View, CA"));
        //}

    }
}

