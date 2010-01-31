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
        ITag GetTagIfExists(string tagString, TagType type);
        ITag GetAndAddTagIfAbsent(string tagString, TagType type);
        List<ITag> GetTagsFromNameValueCollection(NameValueCollection nameVals);
        List<string> GetTagsFromTypeAhead(string query, TagType? type, int count);
    }
}
