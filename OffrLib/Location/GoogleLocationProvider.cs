using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Common;

namespace Offr.Location
{
    public class GoogleLocationProvider : ILocationProvider, IMemCache
    {
        private SortedList<string, ILocation> _knownLocations;

        public GoogleLocationProvider()
        {
            // invalidation during initialization - like opening with a clean slate
            Invalidate();
        }

        public ILocation Parse(string locationSource)
        {
            throw new System.NotImplementedException();
        }

        public void Invalidate()
        {
            _knownLocations = new SortedList<string, ILocation>();
        }
    }
}
