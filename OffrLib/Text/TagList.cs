using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Common;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Repository;

namespace Offr.Text
{
    //FIXME ideally this class would be marked internal - don't use it outside this assembly
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
                foreach (ITag tag in tags)
                {
                    Add(tag);
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
            _list.Add(tag);
            _tagsByType[tag.type].Add(tag);
            if (!_matchTags.Contains(tag.match_tag))
            {
                _matchTags.Add(tag.match_tag);
            }
        }

        public void Clear()
        {
            InitBackingLists();
        }

        public bool Contains(ITag tag)
        {
            return _matchTags.Contains(tag.match_tag);
            // bit faster than this..
            //return _list.Contains(tag);
        }

        public void CopyTo(ITag[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(ITag tag)
        {
            bool removed = (_list.Remove(tag) &&
                            _tagsByType[tag.type].Remove(tag));
            
            // if that was the last of this tag, we don't have it no more..
            if (!_list.Contains(tag))
            {
                _matchTags.Remove(tag.match_tag);
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
            _tagsByType[tag.type].Add(tag); // *think* this is OK not to use the index, because 'by index' stuff is all handled from the _list side of things
        }

        public void RemoveAt(int index)
        {
            ITag existingTag = _list[index];
            _list.RemoveAt(index);
            _tagsByType[existingTag.type].Remove(existingTag);
            // if that was the last of this tag, we don't have it no more..
            if (!_list.Contains(existingTag))
            {
                _matchTags.Remove(existingTag.match_tag);
            }
        }
        [JsonConverter(typeof(TagConverter))]
        public ITag this[int index]
        {
            get { return _list[index]; }
            set 
            {
                ITag existingTag = _list[index];
                _list[index] = value;
                if (existingTag.type == value.type)
                {
                    int indexOfExisting = _tagsByType[existingTag.type].IndexOf(existingTag);
                    _tagsByType[existingTag.type][indexOfExisting] = value;
                }
                else
                {
                    _tagsByType[existingTag.type].Remove(existingTag);
                    _tagsByType[value.type].Add(value);
                }

                if (existingTag.match_tag != value.match_tag)
                {
                    if (!_list.Contains(existingTag))
                    {
                        _matchTags.Remove(existingTag.match_tag);
                    }
                    if (!_matchTags.Contains(value.match_tag))
                    {
                        _matchTags.Add(value.match_tag);
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
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
             List<ITag> tmp = JSON.ReadProperty<List<ITag>>(serializer, reader, "tags");
            TagProvider provider = Global.Kernel.Get<TagProvider>();            
            foreach (ITag tag in tmp)
            {
                _list.Add(provider.GetTag(tag.tag));
            }

        }

        #endregion

        public bool Equals(TagList other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._list.SequenceEqual(_list);
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
    }
}
