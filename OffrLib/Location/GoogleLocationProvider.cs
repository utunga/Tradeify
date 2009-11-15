using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Offr.Message;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Location
{
    public class GoogleLocationProvider : ILocationProvider
    {
        public const string GOOGLE_SEARCH_URI = "http://maps.google.com/maps/geo?q={0}"; 
        private MessageType _forType;
        private readonly List<IRawMessageReceiver> _receivers;

        public GoogleLocationProvider(MessageType forType)
        {
            _forType = forType;
            _receivers = new List<IRawMessageReceiver>();
        }
        
        public GoogleLocationProvider()
            : this(MessageType.offr_test){}

        public ILocation Parse(string addressText)
        {
            string requestURI = string.Format(GOOGLE_SEARCH_URI, HttpUtility.UrlEncode(addressText));
                
            string responseData = WebRequest.RetrieveContent(requestURI);
            GoogleResultSet resultSet = (new JavaScriptSerializer()).Deserialize<GoogleResultSet>(responseData);
            Console.WriteLine(responseData);
            return Location.From(resultSet);
        }

        //public string getStatus()
        //{
        //    string testlocation = "q=1500+Amphitheatre+Parkway,+Mountain+View,+CA";
        //    //string url = String.Format(WebRequest.TWITTER_SEARCH_INIT_URI+testlocation, query);
        //    //string url = String.Format(WebRequest.TWITTER_SEARCH_INIT_URI, query); // query back as far as we can go for these keywords 
        //    string responseData = WebRequest.RetrieveContent(testlocation);
        //    GoogleResultSet resultSet = (new JavaScriptSerializer()).Deserialize<GoogleResultSet>(responseData);
        //    Offr.Location l = new Offr.Location();

        //    return resultSet.Status;

        //}
        //public string getResponseData()
        //{
        //    return WebRequest.RetrieveContent("http://maps.google.com/maps/geo?q=1500+Amphitheatre+Parkway,+Mountain+View,+CA");
        //}
    }
}