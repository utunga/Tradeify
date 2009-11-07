using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public static class DateUtils
    {

#if DEBUG 
        /// <summary>
        /// Datetime to use for testing -  localTime zone
        /// </summary>
        public static DateTime? TestingNow { get; set; } 
#endif

        public static string FriendlyLocalTimeStampFromUTC(DateTime utcDateTime)
        {
            // holding 'now' constant during testing (and lifetime of this method excution)
            DateTime now = TestingNow ?? DateTime.Now;
            
            // localTime is actually 'server local time' of course..
            DateTime localTime = utcDateTime.ToLocalTime();
            TimeSpan timeSince = TimeSpan.FromTicks(now.Ticks -localTime.Ticks);

            if (timeSince.TotalHours < 24 &&
                localTime.DayOfYear == now.DayOfYear) 
            {
                return string.Format("Today, {0:t}", localTime);  //t = '4:48 PM'
            }
            else if (timeSince.TotalDays < 7) // less than 7 days ago
            {
                return localTime.ToString("dd MMM, h:mm tt"); 
            }
            return localTime.ToString("dd MMM yyyy"); 
        }

        public static DateTime UTCDateTimeFromTwitterTimeStamp(string twitterTimeStamp)
        {
            // twitter JSON feed looks like this: 'created_at: "Fri, 06 Nov 2009 23:34:48 +0000"'
            // so the following should work provided twitterTimeStamp stays in the GMT timezone (as specified above)
            return DateTime.Parse(twitterTimeStamp).ToUniversalTime();

            //NB messages from Twitter looks a lot like rfc822 - which according to http://stackoverflow.com/questions/284775/how-do-i-parse-and-convert-datetimes-to-the-rfc-822-date-time-format 
            //   is supported by microsoft's DateTime.Parse implementation. (That is, it implements RFC-1123 which 'supports' rfc822 provided you stay in GMT TimeZone (which we appear to be))
       
        }
    }
}
