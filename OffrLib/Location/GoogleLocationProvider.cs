using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Ninject;
using NLog;
using Offr.Common;
using Offr.Json;
using Offr.Message;
using Offr.Repository;
using Offr.Text;

namespace Offr.Location
{
    public class GoogleLocationProvider : ILocationProvider
    {
        protected ILocationRepository LocationRepository;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        //old urls (v2 of API with generic address lookup (preferring NZ))
        
        //public const string GOOGLE_SEARCH_URI = "http://maps.google.com/maps/geo?q={0}&output=json&oe=utf8&sensor=false&key={1}&gl=NZ"; 
        //public const string GOOGLE_API_KEY = "ABQIAAAABEpdHyPr3QztCREcH5edthQy_El0usyvt1K1GNmivQtTj-_axBQHCZxNbRJdVxkhdKuz2qe7aUF3hQ";
        //public const string GOOGLE_REVERSE_URI = "http://maps.google.com/maps/geo?output=json&oe=utf-8&ll={0}%2C{1}";

        //v3 api - not consistent with our parser or summint
        //southwest->northeast of Canterbury is the following two points:
        //var sw = new GLatLng(-43.64, 172.24);
        //var ne = new GLatLng(-43.315, 172.99);
        //bounds=-43.64,172.24|-43.315,172.99
        //public const string GOOGLE_SEARCH_URI = "http://maps.googleapis.com/maps/api/geocode/json?address={0}&bounds=-43.64,172.24|-43.315,172.99&sensor=false";


        
        public const string GOOGLE_API_KEY = "ABQIAAAABEpdHyPr3QztCREcH5edthQy_El0usyvt1K1GNmivQtTj-_axBQHCZxNbRJdVxkhdKuz2qe7aUF3hQ";
        //v2 style url
        //center of chch ll=-43.489,172.76
        //span for area of chch spn=0.354193,0.854187
        public static string GOOGLE_SEARCH_CHCH_URI = "http://maps.google.com/maps/geo?q={0}&output=json&oe=utf8&sensor=false&key={1}&ll=" + HttpUtility.UrlEncode("-43.489,172.76") + "&spn=" + HttpUtility.UrlEncode("0.354193,0.854187") + "&gl=NZ";
        public static string GOOGLE_SEARCH_FALLBACK_URI = "http://maps.google.com/maps/geo?q={0}&output=json&oe=utf8&sensor=false&key={1}&gl=NZ";  // try for somewhere in NZ, but will give addresses outside of there OK
        //v2 style url with bounds to prefer Canterbury addresses
        public const string GOOGLE_REVERSE_URI = "http://maps.google.com/maps/geo?output=json&oe=utf-8&ll={0}%2C{1}";

        private readonly List<IRawMessageReceiver> _receivers;
        private readonly IWebRequestFactory _webRequest;

        public GoogleLocationProvider(IWebRequestFactory webRequestFactory)
        {
            _webRequest = webRequestFactory;
            _receivers = new List<IRawMessageReceiver>();
            LocationRepository=new LocationRepository();
        }
        
        public virtual ILocation Parse(string addressText)
        {
            return Parse(addressText, null);
        }

        public ILocation ParseFromApproxText(string address)
        {
            //search for a location then trim the array one word at a time
            ILocation best = null;
            while (address.Length >= 1)
            {
                address = address.Trim();
                ILocation newLocation = Parse(address);
                if (newLocation != null)
                {
                    if (best == null) best = newLocation;
                    if (newLocation.Accuracy >= best.Accuracy) best = newLocation;
                }
                Regex endRegex = new Regex("([ |,][^( |,)]+$$)");
                Match endMatch = endRegex.Match(address);
                if (endMatch.Groups.Count > 1)
                {
                    address = address.TrimEnd(endMatch.Groups[0].Value.ToCharArray());
                    address = address.Trim();
                }
                else break;
                //address = address.Substring(0, address.Length - 2);
            }
            return best;
        }

        public virtual ILocation Parse(string addressText, string scopeLocation)
        {
            //FIXME this next line will cause a fail if two locations are simultaneously in use with same addressText (eg Canterbury) but two different scope location 
            ILocation previouslyFound = LocationRepository.Get(addressText);
            if (previouslyFound != null) return previouslyFound;

            GoogleResultSet resultSet = GetResultSet(addressText);
            
            return GetNewLocation(addressText, scopeLocation, resultSet);
        }

        protected ILocation GetNewLocation(string addressText, string scopeLocation, GoogleResultSet resultSet)
        {
            ILocation newlyFound = Location.From(resultSet, scopeLocation);
                        
            if(newlyFound==null) return null;

            //set the AddressText for the ID required by the repository
            SaveLocation(newlyFound, addressText);
            return newlyFound;
        }

        private void SaveLocation(ILocation newlyFound, string addressText)
        {
            newlyFound.AddressText = addressText;
            LocationRepository.Save(newlyFound);
        }
        
        private GoogleResultSet reverseGeocodeCoordinates(string text, GoogleResultSet.PlacemarkType placemark)
        {
            string requestURI = string.Format(GOOGLE_REVERSE_URI, placemark.Point.coordinates[1],placemark.Point.coordinates[0]);
            string responseData = _webRequest.RetrieveContent(requestURI);
            //make sure the name isnt coordinates
            GoogleResultSet resultSet = JSON.Deserialize<GoogleResultSet>(responseData);
            resultSet.name = text;
            return resultSet;
        }


        protected GoogleResultSet GetResultSet(string addressText)
        {
            try
            {
                string requestURI = string.Format(GOOGLE_SEARCH_CHCH_URI, HttpUtility.UrlEncode(addressText), GOOGLE_API_KEY);
                string responseData = _webRequest.RetrieveContent(requestURI);
                GoogleResultSet resultSet = JSON.Deserialize<GoogleResultSet>(responseData);

                if (resultSet.Placemark == null) //failed to parse with boundingbox on Christchurch, fallback to more general
                {
                    
                    requestURI = string.Format(GOOGLE_SEARCH_FALLBACK_URI, HttpUtility.UrlEncode(addressText), GOOGLE_API_KEY);
                    responseData = _webRequest.RetrieveContent(requestURI);
                    resultSet = JSON.Deserialize<GoogleResultSet>(responseData);
                    if (resultSet.Placemark != null && resultSet.Placemark.Length > 0)
                    {
                        GoogleResultSet.PlacemarkType placemark = resultSet.Placemark[0];
                        int accuracy = int.Parse(placemark.AddressDetails.accuracy);
                        if (accuracy == 0) resultSet = reverseGeocodeCoordinates(addressText, placemark);
                    }
                }

                return resultSet;
            }
            catch (Exception ex)
            {
                _log.ErrorException("Failure during parsing of address will just return null", ex);
                GoogleResultSet blankResult = new GoogleResultSet();
                blankResult.name = addressText;
                GoogleResultSet.PlacemarkType dummyPlace=new GoogleResultSet.PlacemarkType();
                dummyPlace.AddressDetails=new GoogleResultSet.PlacemarkType.AddressDetailsType();
                dummyPlace.AddressDetails.accuracy = "0";
                dummyPlace.Point=new GoogleResultSet.PlacemarkType.PointType();
                dummyPlace.Point.coordinates=new decimal[2];
                dummyPlace.Point.coordinates[0] = 0;
                dummyPlace.Point.coordinates[1] = 0;
                blankResult.Placemark= new GoogleResultSet.PlacemarkType[1];
                blankResult.Placemark[0] = dummyPlace;
                //FIXME need to introduce an 'error occured' status..
                //blankResult.Status = GoogleResultSet.StatusType.ERROR;
                return blankResult;
            }
        }
       
    }
}