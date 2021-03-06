﻿using System;
using System.Collections.Generic;
using Offr.Demo;
using Offr.Location;
using Offr.Text;
using Offr.Users;

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
        public static IEnhancedUserPointer User0, User1, User2;
        public static List<IUserPointer> Users;
        public static ILocation Location0, Location1, Location2;
        public static List<string> Offers;
        public static List<ITag> UsedTags;
        
        static MockData()
        {

            MSG_COUNT = 6;
            Offers = new List<string> { "mulch", "car", "vegetables", "garden supplies", "yams", "squash"};
            DateTime fixedStart = DateTime.Now.Subtract(TimeSpan.FromDays(365)).ToUniversalTime();
            User0 = new TwitterUserPointer("utunga" );
            User0.ProfilePicUrl = "http://s3.amazonaws.com/twitter_production/profile_images/82440779/miles_bigger.jpg";

            Location0 = new Location.Location
                            {
                                GeoLat = (decimal)-40.9827820,
                                GeoLong = (decimal)174.9561080,
                                Address = "Paekakariki",
                                Tags = new List<ITag> { (new Tag(TagType.loc, "Paekakariki")),
                                                        (new Tag(TagType.loc, "New Zealand"))/*,
                                                        (new Tag(TagType.loc, "NZ"))*/}

                            };

            User1 = new TwitterUserPointer("utunga");
            User1.ProfilePicUrl = "http://s3.amazonaws.com/twitter_production/profile_images/82440779/miles_bigger.jpg";
            Location1 = new Location.Location
            {
                GeoLat = (decimal)-41.2864800,
                GeoLong = (decimal)174.7762170,
                Address = "Wellington City",
                Tags = new List<ITag> {(new Tag(TagType.loc, "Wellington")),/*Accuracy not high enough for this(new Tag(TagType.loc, "Wellington")),*/
                (new Tag(TagType.loc, "New Zealand"))/*,(new Tag(TagType.loc, "NZ"))*/}
            };

            //http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=Waiheke+Island&sll=-40.985341,174.95394&sspn=0.012424,0.027895&ie=UTF8&ll=-36.79609,175.095978&spn=0.204268,0.44632&t=h&z=12
            User1 = new TwitterUserPointer("shelly");
            User1.ProfilePicUrl = "http://s3.amazonaws.com/twitter_production/profile_images/140759410/avatar_bigger.jpg";

            Location2 = new Location.Location
            {
                GeoLat = (decimal)/*-36.79609 */- 36.7995140,
                GeoLong = (decimal)/*175.09597*/ 175.0960576,
                Address = "Waiheke Island",
                Tags = new List<ITag> { /*(Accuracy not high enough for thisnew Tag(TagType.loc, "Waiheke")),*//*Accuracy not high enough for this(new Tag(TagType.loc, "Auckland")),*/
                (new Tag(TagType.loc, "New Zealand"))/*,(new Tag(TagType.loc, "NZ"))*/}
            };


            User2 = new TwitterUserPointer("utunga");
            User2.ProfilePicUrl = "http://s3.amazonaws.com/twitter_production/profile_images/82440779/miles_bigger.jpg";
          
            // -- setup facets 
            UsedTags= new List<ITag>();
            UsedTags.Add(new Tag(TagType.currency, "cash"));
            UsedTags.Add(new Tag(TagType.currency, "free"));
            UsedTags.Add(new Tag(TagType.currency, "barter"));
            foreach (string offer in Offers)
            {
                string tagText = offer.Replace(" ", "_");
                UsedTags.Add(new Tag(TagType.tag, tagText));
            }
            foreach(ILocation location in new ILocation[] {Location0,Location1,Location2 })
            {
                foreach (ITag locationTag in location.Tags)
                {
                    UsedTags.Add(locationTag);
                }
            }

            //---- set up the actual (raw) messages

            MockRawMessage raw;

            RawMessages = new List<MockRawMessage>();

            //--------- 0

            raw = new MockRawMessage(0)
            {
                Timestamp = fixedStart, // one year ago
                CreatedBy = User0,
                Location = Location0,
                MoreInfoURL = "http://bit.ly/message0Info",
                OfferText = Offers[0] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.group, "ooooby"));
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "swap"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.tag, Offers[0]));
            foreach (ITag locationTag in Location0.Tags)
            {
                raw.Tags.Add(locationTag);
            }
            raw.Text = raw.ToString();
            RawMessages.Add(raw);
           
            //--------- 1

            raw = new MockRawMessage(1)
            {
                Timestamp = fixedStart.AddSeconds((365-40)), // more than a month ago
                CreatedBy = User1,
                Location = Location1,
                MoreInfoURL = "http://bit.ly/message1Info",
                OfferText = Offers[1] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.group, "ooooby"));
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.tag, Offers[1]));
            foreach (ITag locationTag in Location1.Tags)
            {
                raw.Tags.Add(locationTag);
            }
            raw.Text = raw.ToString();
            RawMessages.Add(raw);

            //--------- 2

            raw = new MockRawMessage(2)
            {
                Timestamp = fixedStart.AddDays((365 - 5)),
                CreatedBy = User2,
                Location = Location2,
                MoreInfoURL = "http://bit.ly/message2Info",
                OfferText = Offers[2] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.group, "ooooby"));
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.tag, Offers[2]));
            foreach (ITag locationTag in Location2.Tags)
            {
                raw.Tags.Add(locationTag);
            }
            raw.Text = raw.ToString();
            RawMessages.Add(raw);

            //--------- 3

            raw = new MockRawMessage(3)
            {
                Timestamp = fixedStart.AddDays(363),
                CreatedBy = User0,
                Location = Location0,
                MoreInfoURL = "http://bit.ly/message3Info",
                OfferText = Offers[3] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.group, "ooooby"));
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.tag, "garden_supplies")); 
            foreach (ITag locationTag in Location0.Tags)
            {
                raw.Tags.Add(locationTag);
            }
            raw.Text = raw.ToString();
            RawMessages.Add(raw);

            //--------- 4 

            raw = new MockRawMessage(4)
            {
                Timestamp = fixedStart.AddDays(364.5),
                CreatedBy = User1,
                Location = Location2,
                MoreInfoURL = "http://bit.ly/message4Info",
                OfferText = Offers[4] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.group, "ooooby"));
            raw.Tags.Add(new Tag(TagType.currency, "free"));
            raw.Tags.Add(new Tag(TagType.currency, "barter"));
            raw.Tags.Add(new Tag(TagType.tag, Offers[4]));
            foreach (ITag locationTag in Location2.Tags)
            {
                raw.Tags.Add(locationTag);
            }
            raw.Text = raw.ToString();
            RawMessages.Add(raw);

            //--------- 5

            raw = new MockRawMessage(5)
            {
                Timestamp = fixedStart.AddDays(365),
                CreatedBy = User2,
                Location = Location2,
                MoreInfoURL = "http://bit.ly/message5Info",
                OfferText = Offers[5] + " available now",
                EndByText = null,
                EndBy = null
            };
            raw.Tags.Add(new Tag(TagType.group, "ooooby"));
            raw.Tags.Add(new Tag(TagType.currency, "cash"));
            raw.Tags.Add(new Tag(TagType.tag, Offers[5]));
            foreach (ITag locationTag in Location2.Tags)
            {
                raw.Tags.Add(locationTag);
            }
            raw.Text = raw.ToString();
            RawMessages.Add(raw);

            if (RawMessages.Count != MSG_COUNT)
            {
                throw new ApplicationException("Check the MockData class, wrong number of raw messages being returned");
            }

            Users = new List<IUserPointer>() { User0, User1, User2 };

        }

    }
}
