using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public interface ITagProvider
    {
        ITag GetTag(String tagString, TagType type);
        List<ITag> GetTagsFromNameValueCollection(NameValueCollection nameVals);
    }
}
