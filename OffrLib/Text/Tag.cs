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

        public Tag()
        {
        }


        public override string ToString()
        {
            return match_tag;
        }

        public string ID
        {
            get { throw new NotImplementedException(); }
        }

        public bool Equals(Tag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.type, type) && Equals(other.tag, tag);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Tag)) return false;
            return Equals((Tag) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (type.GetHashCode()*397) ^ (tag != null ? tag.GetHashCode() : 0);
            }
        }
    }
}
