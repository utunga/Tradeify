using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Repository;
using Offr.Text;

namespace Offr.Location
{
    public class Location : ILocation, IEquatable<Location>
    {
        public decimal GeoLat { get; set; }
        public decimal GeoLong { get; set; }
        public string Address { get; set; }
        public string AddressText { get; set; }
        public int Accuracy { get; set; }
        
        private List<ITag> _locationTags { get; set; }
        public IList<ITag> Tags
        {
            get { return _locationTags; }
            set { _locationTags = new List<ITag>(value); }
        }
        public string ID
        {
            get { return AddressText; }
        }
        public Location()
        {
            _locationTags = new List<ITag>();
        }

        #region override Equality methods 

        public bool Equals(Location other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            //nb SequenceEqual relies on ordering being the same
            return  _locationTags.SequenceEqual(other._locationTags) &&
                    other.GeoLat == GeoLat && 
                    other.GeoLong == GeoLong && 
                    Equals(other.Address, Address) ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Location)) return false;
            return Equals((Location) obj);
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

        public override string ToString()
        {
            return string.Format("{0}({1},{2})", AddressText, GeoLat, GeoLong);
        }

        #region static methods

        /// <summary>
      
        /// </summary>
        public static Location From(GoogleResultSet googleResultSet)
        {
            Debug.Assert((googleResultSet != null));
            //make sure there is location data else return null
            if (googleResultSet.Placemark == null || googleResultSet.Placemark [0]== null) return null;
            // At the moment we are just taking the first result as definitieve
            // We might want to do something about multiple matches to the geo code query 
            GoogleResultSet.PlacemarkType placemark = googleResultSet.Placemark[0];
            return From(googleResultSet.name, placemark);
        }
        
        /// <summary>
        /// Copy a GoogleResultSet (serialized from GoogleAPI) into a new Location object
        /// (retaining only the data we actually want in a Location)
        /// 
        /// GoogleResultSets can contain multiple matching Placemark results. Use the scopeLocation (where not null)
        /// to bias the choice of best placemark otherwise just use the first location google provides
        /// </summary>
        public static Location From(GoogleResultSet googleResultSet, string scopeLocation)
        {
            Debug.Assert(googleResultSet != null);
            if (googleResultSet.Placemark == null)
            {
                // no matching results
                return null; 
            }

            Location first=null;
            scopeLocation = Tag.CanonicalizeText(scopeLocation);

            foreach (GoogleResultSet.PlacemarkType placemark in googleResultSet.Placemark)
            {
                Location location = From(googleResultSet.name, placemark);
                if(first==null) 
                    first = location;

                if (scopeLocation != null)
                {
                    //look through the location tags and see if you can find any that match the scope location given
                    foreach (ITag tag in location._locationTags)
                    {
                        if (tag.Text.Contains(scopeLocation)) return location;
                    }
                }
            }
            // otherwise just return the first result
            return first;
        }

        private static Location From(string addressText, GoogleResultSet.PlacemarkType placemark)
        {
            int accuracy = int.Parse(placemark.AddressDetails.accuracy);

            Location loc = new Location
                               {
                                   Address = addressText,
                                   GeoLat = placemark.Point.coordinates[1],
                                   GeoLong = placemark.Point.coordinates[0],
                                   Accuracy = accuracy
                               };
            // not sure whether we should get the latitude and longitude from LatLonBox or the Point field of the json.
            // the longitude appears slightly different in each - going with Point for now
            string streetAddress = null;
            string localityName=null;
            string region = null;
            if (placemark.AddressDetails.Country != null)
            {
                if (placemark.AddressDetails.Country.DependentLocality != null)
                {
                    localityName =
                           placemark.AddressDetails.Country.DependentLocality.DependentLocalityName;
                    if (placemark.AddressDetails.Country.DependentLocality.Thoroughfare != null)
                        streetAddress =
                            placemark.AddressDetails.Country.DependentLocality.Thoroughfare.
                                ThoroughfareName;
                }
                if (placemark.AddressDetails.Country.Locality != null)
                {
                    localityName = placemark.AddressDetails.Country.Locality.LocalityName;
                }
                if (placemark.AddressDetails.Country.AdministrativeArea != null)
                {
                    region = placemark.AddressDetails.Country.AdministrativeArea.AdministrativeAreaName;
                    if (placemark.AddressDetails.Country.AdministrativeArea.Locality != null)
                    {
                        localityName = placemark.AddressDetails.Country.AdministrativeArea.Locality.LocalityName;
                    }
                    if (placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea != null)
                    {
                        if (placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality != null)
                        {
                            localityName =
                                placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.
                                    LocalityName;

                            if (
                                placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.
                                    Thoroughfare != null)
                                streetAddress =
                                    placemark.AddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.
                                        Thoroughfare.ThoroughfareName;
                        }
                    }

                    if (placemark.AddressDetails.Country.AdministrativeArea.DependentLocality != null)
                    {
                        localityName =
                            placemark.AddressDetails.Country.AdministrativeArea.DependentLocality.DependentLocalityName;

                        if (placemark.AddressDetails.Country.AdministrativeArea.DependentLocality.Thoroughfare != null)
                            streetAddress =
                                placemark.AddressDetails.Country.AdministrativeArea.DependentLocality.Thoroughfare.
                                    ThoroughfareName;
                    }
                }
                string countryName = placemark.AddressDetails.Country.CountryName;
                string countryCode = placemark.AddressDetails.Country.CountryNameCode;

                AddTags(countryName, loc, countryCode, region, localityName);

            }
            return loc;
        }

 

        private static void AddTags(string countryName, Location loc, string countryCode, string region, string localityName)
        {
            ITagRepository tagRepository = Global.Kernel.Get<ITagRepository>();

            if (!string.IsNullOrEmpty(countryName))
            {
                ITag tag = tagRepository.GetAndAddTagIfAbsent(countryName, TagType.loc);
                loc.Tags.Add(tag);
            }

            //Dont add Country code for now
            /* if (!string.IsNullOrEmpty(countryCode))
                 loc.Tags.Add(new Tag(TagType.loc, countryCode));*/

            if (!string.IsNullOrEmpty(region))
            {
                ITag tag = tagRepository.GetAndAddTagIfAbsent(region, TagType.loc);
                loc.Tags.Add(tag);
            }

            if (!string.IsNullOrEmpty(localityName))
            {
                ITag tag = tagRepository.GetAndAddTagIfAbsent(localityName, TagType.loc);
                loc.Tags.Add(tag);
            }
        }

        #endregion
        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "geo_lat", GeoLat);
            JSON.WriteProperty(serializer, writer, "geo_long", GeoLong);
            JSON.WriteProperty(serializer, writer, "address", Address);
            JSON.WriteProperty(serializer, writer, "tags", Tags);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            GeoLat=JSON.ReadProperty<decimal>(serializer, reader, "geo_lat");
            GeoLong =JSON.ReadProperty<decimal>(serializer, reader, "geo_long");
            Address=JSON.ReadProperty<string>(serializer, reader, "address");
            Tags=JSON.ReadProperty<IList<ITag>>(serializer, reader, "tags");
        }
    }
        #endregion JSON
}