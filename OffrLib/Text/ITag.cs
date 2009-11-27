using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Json.Converter;
using Offr.Repository;

namespace Offr.Text
{
    public interface ITag : ITopic, ICanJson
    {
        string match_tag { get; }
        TagType type { get; }
        string tag { get; }
        
    }
}