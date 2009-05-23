using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Location
{
    public interface ILocation
    {
        decimal GeoLat { get; }
        decimal GeoLong { get; }
        string SourceText { get; }
        string City { get; }
        string Region { get; }
        string Country { get; }
        string CountryCode { get; }
        IEnumerable<ITag> LocationTags { get; }
    }
}
