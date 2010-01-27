using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;

namespace Offr.Text
{
    public class Tag : ITag, IEquatable<Tag>
    {
        private string _matchTag;
        public string MatchTag
        {
            get { return _matchTag; }
        }

        private TagType _tagType;
        public TagType Type
        {
            get { return _tagType; }
            set
            {
                _tagType = value;
                UpdateMatchTag();
            }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                UpdateMatchTag();
            }
        }

        /// <summary>
        /// Immutable type - you can't change a tag once its created, you have to add/remove a new one
        /// FIXME: should be internal
        /// </summary>
        public Tag(TagType type, string tagText)
        {
            this.Type = type;
            this.Text = Tag.CanonicalizeText(tagText);
        }

        internal Tag()
        {
        }

        public override string ToString()
        {
            return MatchTag;
        }

        public string ID
        {
            get { return Text; }
        }

        #region Equals/GetHashCode

        public bool Equals(Tag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Text, Text);
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
                return (Type.GetHashCode()*397) ^ (Text != null ? Text.GetHashCode() : 0);
            }
        }
        #endregion

        #region JSON

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer,writer,"tag",Text);
            JSON.WriteProperty(serializer, writer, "type", Type);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            /*
                Text = JSON.ReadProperty<string>(serializer, reader, "tag");
                Type = JSON.ReadProperty<TagType>(serializer, reader, "type");
            */
            JSON.ReadAndAssert(reader);
            if (reader.TokenType != JsonToken.PropertyName)
                throw new JsonSerializationException(string.Format("Expected JSON property"));
           
            string typeStr = reader.Value.ToString();
            Type = (TagType)Enum.Parse(typeof(TagType), typeStr, true);
            JSON.ReadAndAssert(reader);
            Text = (string)serializer.Deserialize(reader, typeof(string));
             
        }

        #endregion JSON

        private void UpdateMatchTag()
        {
            _matchTag = Type + "/" + Text;
        }

        public static string CanonicalizeText(string tagText)
        {
            if (tagText == null) return null;
            return tagText.ToLowerInvariant().Replace(" ", "_");
        }

    }
}
