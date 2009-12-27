using System;
using System.Collections.Generic;
using Offr.Location;
using Offr.Text;
using Offr.Twitter;
using Offr.Users;

namespace Offr.Tests
{
    /// <summary>
    /// Basically a class that provides some data laid out in a certain form for testing purposes
    /// (there is a name to jsutify this pattern but can't remember what it is)
    /// </summary>
    public static class DemoData
    {
        public static List<IRawMessage> RawMessages;

        public static string DemoNameSpace
        {
            get { return "DemoData"; }
        }

        static DemoData()
        {

            TwitterUserPointer User1 = new TwitterUserPointer("twollar_test_1");
            User1.ProfilePicUrl = "http://s3.amazonaws.com/twitter_production/profile_images/228862942/YinD_ContactSheet-003_normal.jpg";

            TwitterUserPointer User0 = new TwitterUserPointer("twooooby");
            User0.ProfilePicUrl = "http://s3.amazonaws.com/twitter_production/profile_images/255244783/n581121541_991717_2060_normal.jpg";

            //---- set up the raw messages
            RawMessages = new List<IRawMessage>();

            TwitterMessagePointer msgPointer = new TwitterMessagePointer(1);
            IRawMessage raw = new MockRawMessage("#offr For sale: Kitchen jug. #nzd or #barter L:Paekakariki: http://bit.ly/234 #freecycle", msgPointer, User0, "2009-06-24");
            RawMessages.Add(raw);

            msgPointer = new TwitterMessagePointer(2);
            raw = new MockRawMessage("#offr Huge pile of already split #wood will gladly #barter for whatever you have got L:Paekakariki: http://bit.ly/1231 #barter #wood #ooooby", msgPointer, User0, "2009-06-24");
            RawMessages.Add(raw);
            
            msgPointer = new TwitterMessagePointer(3);
            raw = new MockRawMessage("#offr Everything including the kitchen sink.. old wood, formica, etc. #free to a good home. L:home: #freecycle", msgPointer, User0, "2009-06-24");
            RawMessages.Add(raw);
            
            msgPointer = new TwitterMessagePointer(4);
            raw = new MockRawMessage("#offr Big lemon tree, #free #lemons L:Paekakariki http://bit.ly/1231 #free #lemons #ooooby", msgPointer, User0, "2009-06-24");
            RawMessages.Add(raw);
            
            msgPointer = new TwitterMessagePointer(5);
            raw = new MockRawMessage("#offr Come get some of our beautiful #pumpkin #barter for other veges L:Whitehead bay, Waiheke: http://bit.ly/1234 #waiheke #ooooby", msgPointer, User0, "2009-06-24");
            RawMessages.Add(raw);
            
            msgPointer = new TwitterMessagePointer(6);
            raw = new MockRawMessage("#offr Corn L:55 Wellington Road, Paekakariki: for http://bit.ly/234 #ooooby", msgPointer, User1, "2009-06-24");
            RawMessages.Add(raw);
            
            msgPointer = new TwitterMessagePointer(7);
            raw = new MockRawMessage("#offr Kitchen jug, works great, in L:K Road, Auckland: http://bit.ly/1231 #freecycle", msgPointer, User1, "2009-06-24");
            RawMessages.Add(raw);
            
            msgPointer = new TwitterMessagePointer(8);
            raw = new MockRawMessage("#offr Keen to swap our old fridge for anything smaller #barter in L:Whitehead bay, Waiheke: http://bit.ly/1234 #waiheke #household #freecycle", msgPointer, User1, "2009-06-24");
            RawMessages.Add(raw);

            msgPointer = new TwitterMessagePointer(9);
            raw = new MockRawMessage("#offr For sale: Kitchen jug. #nzd or #barter L:Christchurch, NZ: http://bit.ly/234 #freecycle", msgPointer, User1, "2009-06-24");
            RawMessages.Add(raw);

        }

    }
}
