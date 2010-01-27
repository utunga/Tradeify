using System;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Text;

namespace Offr.Text
{
    public class TagWithCount : IComparable<TagWithCount>, ICanJson
    {
        public ITag tag;
        public int count;

        public int CompareTo(TagWithCount other)
        {
            int compareCount = this.count.CompareTo(other.count);
            return compareCount == 0 ? this.tag.Type.CompareTo(other.tag.Type) : compareCount;
        }

        public override string ToString()
        {
            return tag.MatchTag + ":" + count;
        }

        #region Implementation of ICanJson

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "type", tag.Type.ToString());
            JSON.WriteProperty(serializer, writer, "tag", tag.Text.ToString());
            JSON.WriteProperty(serializer, writer, "count", count);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}