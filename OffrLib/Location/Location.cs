using System;
using System.Collections.Generic;
using System.Diagnostics;
using Offr.Text;

namespace Offr.Location
{
    public class Location : ILocation
    {
        public decimal GeoLat { get; set; }
        public decimal GeoLong { get; set; }
        public string Address { get; set; }
        
        private List<ITag> _locationTags { get; set; }
        public IList<ITag> Tags
        {
            get { return _locationTags; }
            set { _locationTags = new List<ITag>(value); }
        }

        public Location()
        {
            _locationTags = new List<ITag>();
        }

        #region override Equality methods (not sure why)

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Location other = (Location) obj;


            if (other==this) return true; //reference equality saves time (Same object)
            return other.GeoLat == GeoLat && 
                   other.GeoLong == GeoLong && 
                   Equals(other.Address, Address) && 
                   Equals(other._locationTags, _locationTags);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = GeoLat.GetHashCode();
                result = (result*397) ^ GeoLong.GetHashCode();
                result = (result*397) ^ (Address != null ? Address.GetHashCode() : 0);
                result = (result*397) ^ (_locationTags != null ? _locationTags.GetHashCode() : 0);
                return result;
            }
        }

        #endregion


        #region static methods

        /// <summary>
        /// Copy a GoogleResultSet (serialized from GoogleAPI) into a new Location object
        /// (retaining only the data we actually want in a Location)
        /// </summary>
        public static Location From(GoogleResultSet googleResultSet)
        {
                       Debug.Assert((googleResultSet != null));
            //make sure there is location data else return null
            if (googleResultSet.Placemark == null || googleResultSet.Placemark [0]== null) return null;


            // At the moment we are just taking the first result as definitieve
            // We might want to do something about multiple matches to the geo code query 
            GoogleResultSet.PlacemarkType placemark = googleResultSet.Placemark[0];
           
            Location loc = new Location
                               {
                                   Address = googleResultSet.name,
                                   GeoLat = placemark.Point.coordinates[1],
                                   GeoLong = placemark.Point.coordinates[0],
                               };

            // not sure whether we should get the latitude and longitude from LatLonBox or the Point field of the json.
            // the longitude appears slightly different in each - going with Point for now

            string streetAddress = null;
            string localityName=null;
            string region = null;
            
            if (placemark.AddressDetails.Country.Locality!=null)
            {
                localityName = placemark.AddressDetails.Country.Locality.LocalityName;
            }
            if (placemark.AddressDetails.Country.AdministrativeArea != null)
            {
                region = placemark.AddressDetails.Country.AdministrativeArea.AdministrativeAreaName;
                if(placemark.AddressDetails.Country.AdministrativeArea.Locality!=null)
                {
                    localityName = placemark.AddressDetails.Country.AdministrativeArea.Locality.LocalityName;
                }
                if (placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea != null)
                {
                    if (placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality != null)
                    {
                        localityName = placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.LocalityName;

                        if (placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.Thoroughfare != null)
                            streetAddress = placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.Thoroughfare.ThoroughfareName;
                    }
                }

                if (placemark.AddressDetails.Country.AdministrativeArea.DependentLocality != null)
                    {
                        localityName = placemark.AddressDetails.Country.AdministrativeArea.DependentLocality.DependentLocalityName;

                        if (placemark.AddressDetails.Country.AdministrativeArea.DependentLocality.Thoroughfare != null)
                            streetAddress = placemark.AddressDetails.Country.AdministrativeArea.DependentLocality.Thoroughfare.ThoroughfareName;
                    }
            }

            string countryName = placemark.AddressDetails.Country.CountryName;
            string countryCode = placemark.AddressDetails.Country.CountryNameCode;

            if (!string.IsNullOrEmpty(countryName))
                loc.Tags.Add(new Tag(TagType.loc, countryName));

            if (!string.IsNullOrEmpty(countryCode))
                loc.Tags.Add(new Tag(TagType.loc, countryCode));
            
            if (!string.IsNullOrEmpty(region))
                loc.Tags.Add(new Tag(TagType.loc, region));
            
            if (!string.IsNullOrEmpty(localityName))
                loc.Tags.Add(new Tag(TagType.loc, localityName));
            
            // so much more than just the 'thoroughFare name' this is the actual street address - which we don't want for now
            //if (!string.IsNullOrEmpty(streetAddress))
            //    loc.Tags.Add(new Tag(TagType.loc, streetAddress));

            return loc;
        }
        #endregion
    }
}