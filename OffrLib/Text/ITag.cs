using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Repository;

namespace Offr.Text
{
    public interface ITag:ITopic
    {
        string match_tag { get; }
        TagType type { get; }
        string tag { get; }
        
    }
}