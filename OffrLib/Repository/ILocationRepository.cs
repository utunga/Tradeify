using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Location;

namespace Offr.Repository
{
    public interface ILocationRepository
    {
        ILocation Get(string id);
        void Save(ILocation instance);
        void Remove(ILocation instance);
    }
}
