using System;
using System.Collections.Generic;
using Offr.Location;
using Offr.Text;

namespace Offr.Tests
{
    /// <summary>
    /// Basically a class that provides some data laid out in a certain form for testing purposes
    /// (there is a name to jsutify this pattern but can't remember what it is)
    /// </summary>
    public static class MockData
    {
        public static List<MockRawMessage> RawMessages;
        public static int MSG_COUNT;
        public static IUserPointer User0, User1, User2;
        public static ILocation Location0, Location1, Location2;
        public static List<string> Offers;
        public static List<ITag> UsedTags;
        public static List<string> UsedFacets;

        static MockData()
        {
            MSG_COUNT = 6;
            Offers = new List<string> { "mulch", "car", "vegetables", "garden supplies", "yams", "squash"};
            DateTime fixedStart = DateTime.Now.AddSeconds(10);
            User0 = new MockUserPointer("Test", "0");
            Location0 = new Location.Location 
            {
                GeoLat = (decimal)37.0625,
                GeoLong = (decimal)-95.677068,
                SourceText = "Paekakariki",
                City = "Paekakariki",
                Region = "Wellington",
                Country =  "New Zealand",
                CountryCode = "NZ"
            };

            User1 = new MockUserPointer("Test", "1");
            Location1 = new Location.Location
            {
                GeoLat = (decimal)32.0625,
                GeoLong = (decimal)-95.677068,
                SourceText = "Wellington City",
                City = "Wellington",
                Region = "Wellington",
                Country = "New Zealand",
                CountryCode = "NZ"
            };

            //http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=Waiheke+Island&sll=-40.985341,174.95394&sspn=0.012424,0.027895&ie=UTF8&ll=-36.79609,175.095978&spn=0.204268,0.44632&t=h&z=12
            User2 = new MockUserPointer("Test", "2");
            Location2 = new Location.Location
            {
                GeoLat = (decimal)-36.79609,
                GeoLong = (decimal)175.09597,
                City = "Waiheke",
                SourceText = "Waiheke Island",
                Region = "Auckland",
                Country = "New Zealand",
                CountryCode = "NZ"
            };

            // -- setup facets 
            UsedTags= new List<ITag>();
            UsedTags.Add(new Tag(TagType.currency, "cash"));
            UsedTags.Add(new Tag(TagType.currency, "free"));
            UsedTags.Add(new Tag(TagType.currency, "barter"));
            foreach (string offer in Offers)
            {
                UsedTags.Add(new Tag(TagType.hash, offer));
            }
            foreach(ILocation location in new ILocation[] {Location0,Location1,Location2 })
            {
                foreach (ITag locationTag in location.LocationTags)
                {
                    UsedTags.Add(locationTag);
                }
            }

            UsedFacets = new List<string>();
            foreach (ITag tag in UsedTags)
            {
                UsedFacets.Add(tag.match_tag);
            }

            //---- set up the actual (raw) messages

            MockRawMessage raw;

            RawMessages = new List<MockRawMessage>();

            //--------- 0

            raw = new MockRawMessage(0)
            {
                Timestamp = fixedStart.AddSeconds(0).ToString("yyyy-MM-ddThh:mm:ssz"),
                CreatedBy = User0,
                Location = Location0,
                MoreInfoURL = "http://bit.ly/message0Info",
                OfferText = Offers[0] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.hash, Offers[0]));
            foreach (ITag locationTag in Location0.LocationTags)
            {
                raw.Tags.Add(locationTag);
            }
            RawMessages.Add(raw);

           
            //--------- 1

            raw = new MockRawMessage(1)
            {
                Timestamp = fixedStart.AddSeconds(1).ToString("yyyy-MM-ddThh:mm:ssz"),
                CreatedBy = User1,
                Location = Location1,
                MoreInfoURL = "http://bit.ly/message1Info",
                OfferText = Offers[1] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.hash, Offers[1]));
            foreach (ITag locationTag in Location1.LocationTags)
            {
                raw.Tags.Add(locationTag);
            }
            RawMessages.Add(raw);

            //--------- 2

            raw = new MockRawMessage(2)
            {
                Timestamp = fixedStart.AddSeconds(2).ToString("yyyy-MM-ddThh:mm:ssz"),
                CreatedBy = User2,
                Location = Location2,
                MoreInfoURL = "http://bit.ly/message2Info",
                OfferText = Offers[2] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.hash, Offers[2]));
            foreach (ITag locationTag in Location2.LocationTags)
            {
                raw.Tags.Add(locationTag);
            }
            RawMessages.Add(raw);

            //--------- 3

            raw = new MockRawMessage(3)
            {
                Timestamp = fixedStart.AddSeconds(3).ToString("yyyy-MM-ddThh:mm:ssz"),
                CreatedBy = User0,
                Location = Location0,
                MoreInfoURL = "http://bit.ly/message3Info",
                OfferText = Offers[3] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.hash, Offers[3]));
            foreach (ITag locationTag in Location0.LocationTags)
            {
                raw.Tags.Add(locationTag);
            }
            RawMessages.Add(raw);

            //--------- 4 

            raw = new MockRawMessage(4)
            {
                Timestamp = fixedStart.AddSeconds(4).ToString("yyyy-MM-ddThh:mm:ssz"),
                CreatedBy = User1,
                Location = Location2,
                MoreInfoURL = "http://bit.ly/message4Info",
                OfferText = Offers[4] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.hash, Offers[4]));
            foreach (ITag locationTag in Location1.LocationTags)
            {
                raw.Tags.Add(locationTag);
            }
            RawMessages.Add(raw);

            //--------- 5

            raw = new MockRawMessage(5)
            {
                Timestamp = fixedStart.AddSeconds(5).ToString("yyyy-MM-ddThh:mm:ssz"),
                CreatedBy = User2,
                Location = Location2,
                MoreInfoURL = "http://bit.ly/message5Info",
                OfferText = Offers[5] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.currency, "cash"));
            raw.Tags.Add(new Tag(TagType.hash, Offers[5]));
            foreach (ITag locationTag in Location2.LocationTags)
            {
                raw.Tags.Add(locationTag);
            }
            RawMessages.Add(raw);

            if (RawMessages.Count != MSG_COUNT)
            {
                throw new ApplicationException("Check the MockData class, wrong number of raw messages being returned");
            }
        }

    }
}
