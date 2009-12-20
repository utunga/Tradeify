﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Offr.Message;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Location
{
    public class GoogleLocationProvider : ILocationProvider
    {
        protected ILocationRepository LocationRepository;
        public const string GOOGLE_SEARCH_URI = "http://maps.google.com/maps/geo?q={0}"; 
        private MessageType _forType;
        private readonly List<IRawMessageReceiver> _receivers;

        public GoogleLocationProvider(MessageType forType)
        {
            _forType = forType;
            _receivers = new List<IRawMessageReceiver>();
            LocationRepository=new LocationRepository();
        }
        
        public GoogleLocationProvider()
            : this(MessageType.offr_test){}

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
                if (newLocation != null && newLocation.Accuracy != null)
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

        public virtual ILocation Parse(string addressText, string twitterLocation)
        {
            //make sure it can handle twitterLocation ==null
 
            ILocation previouslyFound = LocationRepository.Get(addressText);
            if (previouslyFound != null) return previouslyFound;

            
            GoogleResultSet resultSet = GetResultSet(addressText);

            return GetNewLocation(addressText, twitterLocation, resultSet);
        }

        protected ILocation GetNewLocation(string addressText, string twitterLocation, GoogleResultSet resultSet)
        {
            ILocation newlyFound = (twitterLocation == null) ? Location.From(resultSet) 
                                       : Location.From(resultSet, twitterLocation);
                        
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
            string requestURI = string.Format(GOOGLE_SEARCH_URI, HttpUtility.UrlEncode(addressText));
            string responseData = WebRequest.RetrieveContent(requestURI);
            GoogleResultSet resultSet = (new JavaScriptSerializer()).Deserialize<GoogleResultSet>(responseData);
            return resultSet;
        }
       
    }
}