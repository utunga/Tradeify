using System;
using System.Collections.Generic;
using Offr.Common;
using Offr.Json;
using Offr.Location;
using Offr.Message;
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
        private ILocationProvider _target;

        [TestFixtureSetUp]
        public void SetupTestData()
        {
            _addressToExpectedTags = new SortedList<string, ILocation>();
            _target = new GoogleLocationProvider(new WebRequestFactory());

            string address = "1600 Amphitheatre Parkway, Mountain View, CA";
            ILocation google = new Location.Location
                                   {
                                       GeoLat = (decimal)37.4217590,
                                       GeoLong = (decimal)-122.0843700,
                                       Address = address,
                                       Tags = new List<ITag>
                                                          {
                                                              (new Tag(TagType.loc, "Mountain View")),
                                                              (new Tag(TagType.loc, "CA")),
                                                              (new Tag(TagType.loc, "USA"))/*,
                                                              (new Tag(TagType.loc, "US"))*/
                                                          }
                                   };
            _addressToExpectedTags.Add(address, google);

            address = "30 Fitzroy Street, New Plymouth";
            ILocation fitzroy = new Location.Location
                                    {
                                        GeoLat = (decimal)-39.0443597,
                                        GeoLong = (decimal)174.1080569,
                                        Address = address,
                                        Tags = new List<ITag>
                                                           {
                                                               (new Tag(TagType.loc, "Taranaki")),
                                                               (new Tag(TagType.loc, "Fitzroy")),
                                                               (new Tag(TagType.loc, "New Zealand")) /*,
                                                              (new Tag(TagType.loc, "NZ"))*/
                                                           }
                                    };
            _addressToExpectedTags.Add(address, fitzroy);

            address = "20 Lambton Quay";
            ILocation lambton = new Location.Location
                                    {
                                        GeoLat = (decimal)-41.2786929,
                                        GeoLong = (decimal)174.7785322,
                                        Address = address,
                                        Tags = new List<ITag>
                                                           {
                                                               (new Tag(TagType.loc, "Wellington")),
                                                               (new Tag(TagType.loc, "Wellington Central")),
                                                               (new Tag(TagType.loc, "New Zealand")),/*,
                                                               (new Tag(TagType.loc, "NZ"))*/
                                                           }
                                    };
            _addressToExpectedTags.Add(address, lambton);
            address = "20 Pitt Street,Sydney";
            ILocation sydney = new Location.Location
                                   {
                                       GeoLat = (decimal)-33.816636,
                                       GeoLong = (decimal)150.997453,
                                       Address = address,
                                       Tags = new List<ITag>
                                                          {
                                                              (new Tag(TagType.loc, "New South Wales")),
                                                              (new Tag(TagType.loc, "Sydney")),
                                                              (new Tag(TagType.loc, "Australia"))/*,
                                                              (new Tag(TagType.loc, "AU"))*/
                                                          }
                                   };
            _addressToExpectedTags.Add(address, sydney);
            address = "Sheikh+Zayed+Road,+Dubai,+UAE";

            ILocation uae = new Location.Location
                                {
                                    GeoLat = (decimal) /*25.2286509*/25.0621743,
                                    GeoLong = (decimal) /*55.2876798*/55.1302461,
                                    Address = address,
                                    Tags = new List<ITag>
                                                       {
                                                           (new Tag(TagType.loc, "Dubai")),
                                                           (new Tag(TagType.loc, "Dubai")),
                                                           (new Tag(TagType.loc, "United Arab Emirates"))/*,
                                                           (new Tag(TagType.loc, "AE"))*/
                                                       }
                                };
            _addressToExpectedTags.Add(address, uae);
            address = "30+Rue+Baudin,+Paris,+France";
            ILocation france = new Location.Location
                                   {
                                       GeoLat = (decimal)48.895,
                                       GeoLong = (decimal)2.2520471,
                                       Address = address,
                                       Tags = new List<ITag>
                                                          {
                                                              (new Tag(TagType.loc, "Courbevoie")),
                                                              (new Tag(TagType.loc, "île-de-france")),
                                                              (new Tag(TagType.loc, "France"))/*,
                                                              (new Tag(TagType.loc, "Fr"))*/
                                                          }
                                   };
            _addressToExpectedTags.Add(address, france);
           
            address = "30 Borough Rd, London";
            ILocation uk = new Location.Location
                       {
                           GeoLat = (decimal)51.4988744,
                           GeoLong = (decimal)-0.1018722,
                           Address = address,
                           Tags = new List<ITag>
                              {
                                  (new Tag(TagType.loc, "Camberwell")),
                                  (new Tag(TagType.loc, "Greater London")),
                                  (new Tag(TagType.loc, "UK"))/*,
                                  (new Tag(TagType.loc, "GB"))*/
                              }
                       };

            _addressToExpectedTags.Add(address, uk);




            address = "halswell";
            ILocation halswell = new Location.Location
            {
                GeoLat = (decimal)-43.5854361,
                GeoLong = (decimal)172.5710715,
                Address = address,
                Tags = new List<ITag>
                              {
                                  (new Tag(TagType.loc, "Halswell")),
                                  (new Tag(TagType.loc, "Canterbury")),
                                  (new Tag(TagType.loc, "New Zealand"))
                                  /*,
                                  (new Tag(TagType.loc, "GB"))*/
                              }
            };
            _addressToExpectedTags.Add(address, halswell);

            address = "Dilworth street";
            ILocation dilworth = new Location.Location
            {
                GeoLat = (decimal)-43.5317993,
                GeoLong = (decimal)172.6019896,
                Address = address,
                Tags = new List<ITag>
                              {
                                  
                                  (new Tag(TagType.loc, "Riccarton")),
                                  (new Tag(TagType.loc, "Canterbury")),
                                  (new Tag(TagType.loc, "New Zealand"))/*,
                                  (new Tag(TagType.loc, "GB"))*/
                              }
            };
            _addressToExpectedTags.Add(address, dilworth);

            address = "Saint Martins";
            ILocation stMartins = new Location.Location
            {
                GeoLat = (decimal)-43.555463,
                GeoLong = (decimal)172.6517792,
                Address = address,
                Tags = new List<ITag>
                              {
                                  (new Tag(TagType.loc, "St Martins")),
                                  (new Tag(TagType.loc, "Canterbury")),
                                  (new Tag(TagType.loc, "New Zealand"))/*,
                                  (new Tag(TagType.loc, "GB"))*/
                              }
            };
            _addressToExpectedTags.Add(address, stMartins);
        }

        [Test]
        public void TestDeserialize()
        {
            GoogleResultSet resultSet = JSON.Deserialize<GoogleResultSet>(_testJSON);
            Assert.AreEqual("1500 Amphitheatre Parkway, Mountain View, CA", resultSet.name, "name did not serialize correctly");
        }

        [Test]
        public void TestLiveGoogleParse()
        {
            foreach (string address in _addressToExpectedTags.Keys)
            {
                ILocation location = _target.Parse(address);
                Console.Out.WriteLine("Parsed:");
                Console.Out.WriteLine(JSON.Serialize(location));

                ILocation expected = _addressToExpectedTags[address];
                AssertLocationEquality(address, expected, location);
            }

        }

        //[Test]
        //public void TestLiveGoogleParseWithTwitterLocation()
        //{
        //    String address = "30 Borough Rd";
        //    ILocation nonSpecific = new Location.Location
        //    {
        //        GeoLat = (decimal)51.4989035,
        //        GeoLong = (decimal)-0.101,
        //        Address = address,
        //        Tags = new List<ITag>
        //                      {
        //                          (new Tag(TagType.loc, "Camberwell")),
        //                          (new Tag(TagType.loc, "Greater London")),
        //                          (new Tag(TagType.loc, "United Kingdom"))/*,
        //                          (new Tag(TagType.loc, "GB"))*/
        //                      }
        //    };

        //    // Normally the result london is not the first result for this query
        //    ILocation location = _target.Parse(address, "Greater London");
        //    ILocation expected = nonSpecific;
        //    AssertLocationEquality(address, expected, location);
        //}


        private static void AssertLocationEquality(string forAddress, ILocation expected, ILocation actual)
        {
            //because of the fact that google coordinates like moving around for some reason lower the precision
            Assert.AreEqual(expected.Address, actual.Address, "Address for '" + forAddress + "' was not as expected");

            foreach (ITag locationTag in expected.Tags)
            {

                if (!actual.Tags.Contains(locationTag))
                {
                    Console.Out.WriteLine("Expected:");
                    Console.Out.WriteLine(JSON.Serialize(expected));
                    Console.Out.WriteLine("Actual:");
                    Console.Out.WriteLine(JSON.Serialize(actual));
                }
                Assert.That(actual.Tags.Contains(locationTag), "Expected tag " + locationTag + " was not contained in result for " + forAddress);
            }

            foreach (ITag locationTag in actual.Tags)
            {
                Assert.That(expected.Tags.Contains(locationTag), locationTag + " unexpectedly contained in result for " + forAddress);
            }
            Assert.AreEqual(expected.Tags.Count, actual.Tags.Count, "Somehow, inexplicably, the counts for expected vs actual location tags are different even though they contain the exact same set of tags");

            Assert.AreEqual(Decimal.Parse(expected.GeoLat.ToString().Substring(0, 6)), Decimal.Parse(actual.GeoLat.ToString().Substring(0, 6)), "GeoLat for '" + forAddress + "' was not as expected");
            Assert.AreEqual(Decimal.Parse(expected.GeoLong.ToString().Substring(0, 6)), Decimal.Parse(actual.GeoLong.ToString().Substring(0, 6)), "GeoLong for '" + forAddress + "' was not as expected");
            
        }

        [Test]
        public void TestNonStrictLColon()
        {

            // a specific real example for which i know the query was failing
            ILocation location = _target.ParseFromApproxText("Paekakariki for #free http://bit.ly/message0Info pic http://twitpic.com/r5aon #mulch");
            Assert.AreEqual("Paekakariki", location.AddressText);
        }

        [Test]
        public void TestLandMarkLocation()
        {
            String address = "Little Oneroa";
            ILocation expected = new Location.Location
            {
                GeoLat = (decimal)-36.7845550,
                GeoLong = (decimal)175.027010,
                Address = address,
                Tags = new List<ITag>
                              {
                                  (new Tag(TagType.loc, "New Zealand")),
                                  (new Tag(TagType.loc, "Auckland")),
                                  (new Tag(TagType.loc, "Oneroa")),
                                  //(new Tag(TagType.loc, "United Kingdom"))/*,
                                  //(new Tag(TagType.loc, "GB"))*/
                              }
            };
            ILocation location = _target.Parse(address);
            AssertLocationEquality(address, expected, location);
        }
    }
}
