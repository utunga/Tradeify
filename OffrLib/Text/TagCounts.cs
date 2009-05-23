using System.Collections.Generic;

namespace Offr.Text
{
    public class TagCounts
    {
        public SortedList<string, TagWithCount> Tags { get; set; }
        public int Total { get; set; }
    }
}