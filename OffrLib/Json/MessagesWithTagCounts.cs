using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace Offr.Json
{
    /// <summary>
    /// Wrapper class used during serialization, wraps both messages and their tag counts together
    /// </summary>
    public class MessagesWithTagCounts // : ICanJson //only need to override 'icanjson' if you actually wanna do a manual writejson
    {
        public int MessageCount { get; private set; }
        public int TagCount { get; private set; }
        public IEnumerable<IMessage> Messages { get; private set; }
        public List<TagWithCount> Tags { get; private set; }
        
        public MessagesWithTagCounts(IEnumerable<IMessage> messages)
        {
            //immediately processs message source and count the number of instances of each tag
            TagDex index = new TagDex(messages);
            TagCounts tagCounts = index.GetTagCounts();
         
            Tags = tagCounts.Tags;
            // results.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            //sort by count descending
            Tags.Sort((a, b) => b.count.CompareTo(a.count));
            TagCount = tagCounts.Tags.Count;

            Messages = messages;
            MessageCount = tagCounts.Total;
        }

        //#region Implementation of ICanJson

        //public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        //{
        //}

        //public void ReadJson(JsonReader reader, JsonSerializer serializer)
        //{
        //    throw new NotImplementedException("This class is for transient use only, don't expect you would ever need to deserialize into this class, so that is not supported");
        //}

        //#endregion
    }
}
