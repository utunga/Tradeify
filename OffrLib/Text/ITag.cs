using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public interface ITag
    {
        string match_tag { get; }
        TagType type { get; }
        string tag { get; }
    }
}