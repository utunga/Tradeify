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
        public IList<ITag> LocationTags
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

            //NOTE2J - return 'bool' not Boolean from Equals
            //NOTE2J - List equality should work provided elements within the list also implement equals

            if (other==this) return true; //reference equality saves time (Same object)
            return other.GeoLat == GeoLat && 
                   other.GeoLong == GeoLong && 
                   Equals(other.Address, Address) && 
                   Equals(other._locationTags, _locationTags);
        }

        public override int GetHashCode()
        {
            //NOTE2J - if yuo override equals you should also ovveride GetHashCode()
            // the following code was generated by Resharper
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
            // NOTE2J this line should throw an exception if googleResultSet is null - 'early fail' is good for keeping things simple
            Debug.Assert((googleResultSet != null));

            Location loc = new Location
            {
                GeoLat = googleResultSet.Placemark[0].Point.coordinates[1],
                GeoLong = googleResultSet.Placemark[0].Point.coordinates[0],
                Address = googleResultSet.name,
            };

            //Im not sure whether we should get the latitude and longitude from here or the point field of the json.
            // the longitude appears slightly different 
            //if (googleResultSet.ExtendedData == null || googleResultSet.ExtendedData.LatLonBox ==null) return tmp;

            // At the moment we are just taking the first result as definitieve
            // We might want to do something about multiple matches to the geo code query 
            //
            // FIXME: Check for Dependant Locality! ?what?
            string City = googleResultSet.Placemark[0].AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.LocalityName;
            string Country = googleResultSet.Placemark[0].AddressDetails.Country.CountryName;
            string Region = googleResultSet.Placemark[0].AddressDetails.Country.AdministrativeArea.AdministrativeAreaName;
            string CountryCode = googleResultSet.Placemark[0].AddressDetails.Country.CountryNameCode;

            if (City != null)
                loc.LocationTags.Add(new Tag(TagType.loc, City));
            if (Region != null)
                loc.LocationTags.Add(new Tag(TagType.loc, Region));
            if (Country != null)
                loc.LocationTags.Add(new Tag(TagType.loc, Country));
            if (CountryCode != null)
                loc.LocationTags.Add(new Tag(TagType.loc, CountryCode));

            return loc;
        }
        #endregion
    }
}