using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Common
{
    public interface IMemCache
    {
        void Invalidate();
    }
}