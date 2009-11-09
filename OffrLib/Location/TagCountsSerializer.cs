﻿using System;
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
                                
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(TagCounts) })); }
        }
       
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            TagCounts tagCounts = obj as TagCounts;
            if (tagCounts != null)
            {
                // Create the representation.
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("total", tagCounts.Total);

                ArrayList overall = new ArrayList();

                tagCounts.Tags.Sort();
                tagCounts.Tags.Reverse();

                foreach (TagWithCount tagCount in tagCounts.Tags)
                {
                    Dictionary<string, object> dict =
                        new Dictionary<string, object>()
                            {
                                {"type", tagCount.tag.type.ToString()},
                                {"tag", tagCount.tag.tag},
                                {"count", tagCount.count},
                                {"pct", Math.Floor(((double)tagCount.count/(double)tagCounts.Total) * 100)}
                            };
                    overall.Add(dict);
                }

                result["overall"] = overall;
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