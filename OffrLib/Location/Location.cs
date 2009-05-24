using System.Collections.Generic;
using Offr.Text;

namespace Offr.Location
{
    public class Location : ILocation
    {
        public decimal GeoLat { get; set; }
        public decimal GeoLong { get; set; }
        public string SourceText { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }

        public IEnumerable<ITag> LocationTags
        {
            get { 
                yield return new Tag(TagType.loc, City);
                yield return new Tag(TagType.loc, Region);
                yield return new Tag(TagType.loc, Country);
                yield return new Tag(TagType.loc, CountryCode);
            }
        }
    }
}