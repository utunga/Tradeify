using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Offr.Message;
using Offr.Text;
using Offr.Users;

namespace Offr.Json
{
    public class TagCountsSerializer : JavaScriptConverter
    {
        const int MAX_TAGS_OF_TYPE = 10;
                                
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(List<TagWithCount>) })); }
        }
       
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IEnumerable<TagWithCount> tags = obj as IEnumerable<TagWithCount>;
            if (tags != null)
            {
                // Create the representation.
                Dictionary<string, object> result = new Dictionary<string, object>();
                
                ArrayList overall = new ArrayList();

                SortedList<TagType,int> counters = new SortedList<TagType, int>();
                foreach (TagWithCount tagCount in tags)
                {
                    TagType type = tagCount.tag.Type;
                    counters[type] = counters.ContainsKey(type) ? counters[type] + 1 : 0;
                    if (counters[type] < MAX_TAGS_OF_TYPE)
                    {
                        Dictionary<string, object> dict =
                            new Dictionary<string, object>()
                                {
                                    {"type", type.ToString()},
                                    {"tag", tagCount.tag.Text},
                                    {"count", tagCount.count}
                                };
                        overall.Add(dict);
                    }

                }

                result["tags"] = overall;
                return result;
            }
            // blank return, shouldn't happen
            return new Dictionary<string, object>();
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw  new NotSupportedException("SOrry this is a one way converter currently");
        }

    }
}