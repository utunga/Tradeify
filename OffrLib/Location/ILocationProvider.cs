using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Location
{
    public interface ILocationProvider
    {
        ILocation Parse(string locationSource);
    }
}
