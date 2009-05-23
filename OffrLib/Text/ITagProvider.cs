using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public interface ITagProvider
    {
        ITag FromString(string match_tag);
    }
}
