using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Location
{
    public interface ILocation
    {
        string Address { get; }
        IList<ITag> Tags { get; }
        decimal GeoLat { get; }
        decimal GeoLong { get; }

    }
}
