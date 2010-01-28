using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NLog;
using Offr.Json;
using Offr.Message;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Location
{
    public class GoogleLocationProvider : ILocationProvider
    {
        protected ILocationRepository LocationRepository;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public const string GOOGLE_API_KEY= "ABQIAAAABEpdHyPr3QztCREcH5edthQy_El0usyvt1K1GNmivQtTj-_axBQHCZxNbRJdVxkhdKuz2qe7aUF3hQ";
        public const string GOOGLE_SEARCH_URI = "http://maps.google.com/maps/geo?q={0}&output=json&oe=utf8&sensor=false&key={1}";

       private readonly List<IRawMessageReceiver> _receivers;

        public GoogleLocationProvider()
        {
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

        protected GoogleResultSet GetResultSet(string addressText)
        {
            try
            {
                string requestURI = string.Format(GOOGLE_SEARCH_URI, HttpUtility.UrlEncode(addressText), GOOGLE_API_KEY);
                string responseData = WebRequest.RetrieveContent(requestURI);
                GoogleResultSet resultSet = JSON.Deserialize<GoogleResultSet>(responseData);
                return resultSet;
            }
            catch (System.Net.WebException ex)
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