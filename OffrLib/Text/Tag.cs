using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public class Tag : ITag, IEquatable<Tag>
    {
        public string match_tag
        {
            get { return type + "/" + tag; }
            //set { //parse match_tag and split it }
        }

        public TagType type { get; private set; }
        public string tag { get; private set; }

        /// <summary>
        /// Immutable type - you can't change a tag once its created, you have to add/remove a new one
        /// 
        /// FIXME: should be marked internal
        /// </summary>
        public Tag(TagType type, string tag)
        {
            this.type = type;
            this.tag = tag.ToLowerInvariant();
        }

        //IEquatable<Tag>.equals
        public bool Equals(Tag other)
        {
            return other != null && (match_tag.Equals(other.match_tag));
        }

        // object.equals
        public override bool Equals(object obj)
        {
            Tag other = obj as Tag;
            return other != null && (match_tag.Equals(other.match_tag));
        }

        // object hashCode (must coordinate with above)
        public override int GetHashCode()
        {
            return match_tag.GetHashCode();
        }

        public override string ToString()
        {
            return match_tag;
        }
    }
}
