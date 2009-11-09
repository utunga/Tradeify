using System.Collections.Generic;
using System.Web.Script.Serialization;
using Offr.Location;
using Offr.Text;
using NUnit.Framework;

namespace Offr.Tests
{
    [TestFixture]
    public class TestGoogleLocation
    {
      
        #region test json data
        
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
        public void SetupTestData()
        {
            _addressToExpectedTags = new SortedList<string, ILocation>();

            string address = "1600 Amphitheatre Parkway, Mountain View, CA";
            ILocation google = new Location.Location
            {
               GeoLat = (decimal) 37.4217590,
               GeoLong = (decimal) -122.0843700,
               Address = address,
               LocationTags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "Mountain View")),
                                      (new Tag(TagType.loc, "CA")),
                                      (new Tag(TagType.loc, "USA")),
                                      (new Tag(TagType.loc, "US"))
                                  }
            };
            _addressToExpectedTags.Add(address, google);

            address = "30 Fitzroy Street, New Plymouth";
            ILocation fitzroy = new Location.Location
            {
                GeoLat = (decimal)-39.0443597,
                GeoLong = (decimal)174.1080569,
                Address = address,
                LocationTags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "Taranaki")),
                                      (new Tag(TagType.loc, "Fitzroy")),
                                      (new Tag(TagType.loc, "New Zealand")),
                                      (new Tag(TagType.loc, "NZ"))
                                  }
            };
            _addressToExpectedTags.Add(address, fitzroy);

           address = "20 Lambton Quay";
           ILocation lambton = new Location.Location
           {
               GeoLat = (decimal)-41.2787026,
               GeoLong = (decimal)174.7785408,
               Address = address,
               LocationTags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "Wellington")),
                                      (new Tag(TagType.loc, "Pipitea")),
                                      (new Tag(TagType.loc, "New Zealand")),
                                      (new Tag(TagType.loc, "NZ"))
                                  }
           };
           _addressToExpectedTags.Add(address, lambton);
           address = "20 Pitt Street Mall,Sydney";
           ILocation sydney = new Location.Location
           {
               GeoLat = (decimal)-33.8707700,
               GeoLong = (decimal)151.2082180,
               Address = address,
               LocationTags = new List<ITag>
                                  {
                                      (new Tag(TagType.loc, "NSW")),
                                      (new Tag(TagType.loc, "Sydney")),
                                      (new Tag(TagType.loc, "Australia")),
                                      (new Tag(TagType.loc, "AU"))
                                  }
           };
           //_addressToExpectedTags.Add(address, sydney);
        }

        [Test]
        public void TestCorrectTagsAreRetrieved()
        {

        }

        [Test]
        public void TestDeserialize()
        {
            GoogleResultSet resultSet = (new JavaScriptSerializer()).Deserialize<GoogleResultSet>(_testJSON);
            Assert.AreEqual("1500 Amphitheatre Parkway, Mountain View, CA", resultSet.name, "name did not serialize correctly");           
        }

        [Test]
        public void TestLiveGoogleParse()
        {
            // NOTE2J - renamed 'g' to 'locationProvider' don't ever, ever, *ever* call a variable a single letter - except 'i' and 'j' for loop counters, maybe
            GoogleLocationProvider locationProvider = new GoogleLocationProvider();
            foreach (string address in _addressToExpectedTags.Keys)
            {
                ILocation location = locationProvider.Parse(address);
                ILocation expected = _addressToExpectedTags[address];
                AssertLocationEquality(address, expected, location);
            }

        }

        
        private static void AssertLocationEquality(string forAddress, ILocation expected, ILocation actual)
        {
            //NOTE2J - do it this way you get much more useful debug data
            //         and also save your self the trouble of having to override equals() etc etc..

            Assert.AreEqual(expected.GeoLat, actual.GeoLat, "GeoLat for '" + forAddress + "' was not as expected");
            Assert.AreEqual(expected.GeoLong, actual.GeoLong, "GeoLong for '" + forAddress + "' was not as expected");
            Assert.AreEqual(expected.Address, actual.Address, "Address for '" + forAddress + "' was not as expected");

            foreach (ITag locationTag in expected.LocationTags)
            {
                Assert.That(actual.LocationTags.Contains(locationTag), "Expected tag " + locationTag + " was not contained in result for " + forAddress);    
            }

            foreach (ITag locationTag in actual.LocationTags)
            {
                Assert.That(expected.LocationTags.Contains(locationTag), locationTag + " unexpectedly contained in result for " + forAddress);
            }

            // this would never happen.. but fwiw
            Assert.AreEqual(expected.LocationTags.Count, actual.LocationTags.Count, "Somehow, inexplicably, the counts for expected vs actual location tags are different even though they contain the exact same set of tags" );
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

