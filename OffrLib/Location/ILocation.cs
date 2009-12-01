using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json.Converter;
using Offr.Repository;
using Offr.Text;

namespace Offr.Location
{
    public interface ILocation : ICanJson, ITopic
    {
        string Address { get; }
        IList<ITag> Tags { get; }
        decimal GeoLat { get; }
        decimal GeoLong { get; }
        string AddressText { get; set; }
        int Accuracy
        { get;
            set;
        }
    }
}
