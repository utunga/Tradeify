using System;
using System.Collections.Generic;
using Offr.Text;

namespace Offr.Location
{
    public class Location : ILocation
    {
        public decimal GeoLat { get; set; }
        public decimal GeoLong { get; set; }
        public string SourceText { get; set; }
        //public string City { get; set; }
        //public string Region { get; set; }
        //public string Country { get; set; }
        //public string CountryCode { get; set; }

        public List<ITag> _locationTags { get; set; }

        public Location() { }
        public IEnumerable<ITag> LocationTags
        {
            get
            {
                yield return _locationTags[0];
                yield return _locationTags[1];
                yield return _locationTags[2];
                yield return _locationTags[3];
            }
        }

        public Boolean Equals(ILocation l)
        {
            if (this.GeoLat != l.GeoLat ||
            this.GeoLong != l.GeoLong ||
            this.SourceText != l.SourceText ) return false;
                //|| !this._locationTags.Equals(l.LocationTags
            if(_locationTags.Count!=l._locationTags.Count) return false;
            for (int i = 0; i < this._locationTags.Count; i++)
            {
                if (!_locationTags[i].Equals(l._locationTags[i])) return false;
            }

            return true;

        }
        public static Location From(GoogleResultSet googleResultSet)
        {
            //copy google location properties over into a new Location object
            Location tmp = new Location();

            //Im not sure whether we should get the latitude and longitude from here or the point field of the json.
            // the longitude appears slightly different 
            if (googleResultSet == null) return tmp;
            //if (googleResultSet.ExtendedData == null || googleResultSet.ExtendedData.LatLonBox ==null) return tmp;
            tmp.GeoLat = googleResultSet.Placemark[0].Point.coordinates[1];
            tmp.GeoLong = googleResultSet.Placemark[0].Point.coordinates[0];
            tmp.SourceText = googleResultSet.name;
            tmp._locationTags = new List<ITag>();
            //We might want to do something about multiple matches to the geo code query at the moment we are taking 
            //the first result
            //Check for Dependant Locality!
            //else if(googleResultSet.Placemark[0].AddressDetails.Country.AdministrativeArea.SubAdministrativeArea!=null)
            string City = googleResultSet.Placemark[0].AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.LocalityName;
            string Country = googleResultSet.Placemark[0].AddressDetails.Country.CountryName;
            string Region = googleResultSet.Placemark[0].AddressDetails.Country.AdministrativeArea.AdministrativeAreaName;
            string CountryCode = googleResultSet.Placemark[0].AddressDetails.Country.CountryNameCode;
            
            tmp._locationTags.Add(new Tag(TagType.loc, City));
            tmp._locationTags.Add(new Tag(TagType.loc, Region));
            tmp._locationTags.Add(new Tag(TagType.loc, Country));
            tmp._locationTags.Add(new Tag(TagType.loc, CountryCode));
            //TODO: copy result set data into newly formatted Location class
            return tmp;
        }

    }
}