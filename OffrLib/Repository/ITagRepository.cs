using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Repository
{
    public interface ITagRepository
    {
        ITag GetTag(string tagString, TagType type);
        List<ITag> GetTagsFromNameValueCollection(NameValueCollection nameVals);
    }
}
