using System;
using Offr.Text;

namespace Offr.Text
{
    public class TagWithCount : IComparable<TagWithCount> 
    {
        public ITag tag;
        public int count;

        public int CompareTo(TagWithCount other)
        {
            int compareCount = this.count.CompareTo(other.count);
            return compareCount == 0 ? this.tag.type.CompareTo(other.tag.type) : compareCount;
        }

        public override string ToString()
        {
            return tag.match_tag + ":" + count;
        }
    }
}