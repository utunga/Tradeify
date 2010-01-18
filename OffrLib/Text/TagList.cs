using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Common;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Repository;

namespace Offr.Text
{
    public class TagList : IList<ITag>, ICanJson, IEquatable<TagList>
    {
        private IList<ITag> _list;
        private Dictionary<TagType, List<ITag>> _tagsByType;
        private HashSet<string> _matchTags;

        public TagList() : base()
        {
            InitBackingLists();
        }

        public TagList(IEnumerable<ITag> tags)
            : base()
        {
            InitBackingLists();
            if (tags != null)
            {
                foreach (ITag tag in tags)
                {
                    Add(tag);
                }
            }
        }

        /// <summary>
        /// the reason for this method - an index into the tags of a particular type
        /// FIXME: should be internal, not public
        /// </summary>
        public ReadOnlyCollection<ITag> TagsOfType(TagType tagType)
        {
            return _tagsByType[tagType].AsReadOnly();
        }
   
        public IEnumerator<ITag> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #region implementation of ICollection<ITag>
        
        public void Add(ITag tag)
        {
            //only one of the same tag
            if (!_list.Contains(tag))
            {
                _list.Add(tag);
                _tagsByType[tag.Type].Add(tag);
                if (!_matchTags.Contains(tag.MatchTag))
                {
                    _matchTags.Add(tag.MatchTag);
                }
            }
            
        }

        public void Clear()
        {
            InitBackingLists();
        }

        public bool Contains(ITag tag)
        {
            return _list.Contains(tag);
        }

        public bool Contains(string matchTag)
        {
            return _matchTags.Contains(matchTag);
        }

        public void CopyTo(ITag[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ITag tag)
        {
            bool removed = (_list.Remove(tag) &&
                            _tagsByType[tag.Type].Remove(tag));
            
            // if that was the last of this tag, we don't have it no more..
            if (!_list.Contains(tag))
            {
                _matchTags.Remove(tag.MatchTag);
            }
            return removed;
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region implementation of IList<ITag> interface

        public int IndexOf(ITag tag)
        {
            return _list.IndexOf(tag);
        }

        public void Insert(int index, ITag tag)
        {
            _list.Insert(index,tag);
            _tagsByType[tag.Type].Add(tag); // *think* this is OK not to use the index, because 'by index' stuff is all handled from the _list side of things
        }

        public void RemoveAt(int index)
        {
            ITag existingTag = _list[index];
            _list.RemoveAt(index);
            _tagsByType[existingTag.Type].Remove(existingTag);
            // if that was the last of this tag, we don't have it no more..
            if (!_list.Contains(existingTag))
            {
                _matchTags.Remove(existingTag.MatchTag);
            }
        }
        public ITag this[int index]
        {
            get { return _list[index]; }
            set 
            {
                ITag existingTag = _list[index];
                _list[index] = value;
                if (existingTag.Type == value.Type)
                {
                    int indexOfExisting = _tagsByType[existingTag.Type].IndexOf(existingTag);
                    _tagsByType[existingTag.Type][indexOfExisting] = value;
                }
                else
                {
                    _tagsByType[existingTag.Type].Remove(existingTag);
                    _tagsByType[value.Type].Add(value);
                }

                if (existingTag.MatchTag != value.MatchTag)
                {
                    if (!_list.Contains(existingTag))
                    {
                        _matchTags.Remove(existingTag.MatchTag);
                    }
                    if (!_matchTags.Contains(value.MatchTag))
                    {
                        _matchTags.Add(value.MatchTag);
                    }
                }
            }
        }
        #endregion

        #region private methods

        private void InitBackingLists()
        {
            _list = new List<ITag>();
            _matchTags = new HashSet<string>();
            _tagsByType = new Dictionary<TagType, List<ITag>>();
            foreach (TagType type in Enums.Get<TagType>())
            {
                _tagsByType[type] = new List<ITag>();
            }
        }
        #endregion

        #region JSON

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "tags", _list);

            //foreach (TagType type in _tagsByType.Keys)
            //{
            //    foreach (ITag tag in _tagsByType[type])
            //    {
            //        JSON.WriteProperty(serializer, writer, tag.Type.ToString(), tag.Text);  
            //    }
            //}
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {

            ITagRepository provider = Global.Kernel.Get<ITagRepository>(); //FIXME gotta figure out if this is correct thing to happen
            List<ITag> temp = JSON.ReadProperty<List<ITag>>(serializer, reader, "tags");
            foreach (ITag tag in temp)
            {
                _list.Add(provider.GetAndAddTagIfAbsent(tag.Text, tag.Type));
                _matchTags.Add(tag.MatchTag);
            }
            

            // you can also do this to get:
            //"tags": {
            //  "tag": "offr_test",
            //  "tag": "free",
            //  "tag": "barter",
            //  "tag": "mulch",
            //  "loc": "new zealand",
            //  "loc": "nz",
            //  "loc": "paekakariki"
            //},
            //JSON.ReadAndAssert(reader);
            //JsonToken nextToken = reader.TokenType;
            //while (nextToken == JsonToken.PropertyName)
            //{
            //    string typeStr = reader.Value.ToString();

            //    TagType type = (TagType) Enum.Parse(typeof (TagType), typeStr, true);
            //    JSON.ReadAndAssert(reader);
            //    string tagText = (string) serializer.Deserialize(reader, typeof (string));

            //    _list.Add(provider.GetAndAddTagIfAbsent(tagText, type));

            //    JSON.ReadAndAssert(reader);
            //    nextToken = reader.TokenType;
            //}
           
        }

        #endregion

        public bool Equals(TagList other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            //check that lists are equivalent
            return _list.Intersect(other._list).Count() == _list.Count();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TagList)) return false;
            return Equals((TagList) obj);
        }

        public override int GetHashCode()
        {
            return (_list != null ? _list.GetHashCode() : 0);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("tags:");
            foreach (ITag tag in _list)
            {
                sb.Append(tag.MatchTag);
                sb.Append(",");
            }
            return sb.ToString();
        }
    }
}
