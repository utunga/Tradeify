using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace Offr.Text
{
    public interface IResolvedURI
    {
        string CanonicalURI { get; }
        string OriginalURL { get; }
        Uri URL { get; }
    }
}
