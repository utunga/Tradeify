using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json.Converter;
using Offr.Text;

namespace Offr.Message
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseMessage : IMessage, IEquatable<BaseMessage>
    {
        protected TagList _tags;

        private MessageType _messageType;

        private IRawMessage _source;

        private DateTime _timestamp = DateTime.MinValue;

        [JsonProperty]
        public bool IsValid { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(IUserPointerConverter))]
        public IUserPointer CreatedBy { get; set; }

        [JsonProperty]
        public string MoreInfoURL { get; set; }

        [JsonProperty]
        public IEnumerable<ITag> Tags
        {
            get { return _tags; }
            private set { _tags = new TagList(value); }
        }

        [JsonProperty]
        public MessageType MessageType
        {
            get { return _messageType; }
            internal set
            {
                /*  if (value != ExpectedMessageType)
                  {
                      throw new MessageParseException("Cannot set message type to " + value + " on a " + GetType().Name + ". Expected " + ExpectedMessageType);
                  }*/
                _messageType = value;
            }
        }

        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            private set { _timestamp = value;}
        }

        [JsonProperty]
        [JsonConverter(typeof(RawMessageConverter))]
        public IRawMessage Source 
        { 
            get 
            {
                return _source;
            }
            set
            {
                _source = value;
                _timestamp = DateTime.MinValue; 
                if(value!=null)
                    DateTime.TryParse(_source.Timestamp, out _timestamp);
            }
        }

        #region read only properties
        
        public string ID
        {
            get
            {
                if (_source != null) if (_source.Pointer != null) return _source.Pointer.MatchTag;
                return null;
            }
        }

        public ReadOnlyCollection<ITag> HashTags
        {
            get { return _tags.TagsOfType(TagType.tag); }
        }

        public ReadOnlyCollection<ITag> CommunityTags
        {
            get { return _tags.TagsOfType(TagType.group); }
        }
        
        #endregion

        protected BaseMessage()
        {
            _tags = new TagList();
        }

        /// <summary>
        /// Implementing classes should supply The MessageType they expect for their class and the base will throw a parse expection if it doesn't match
        /// </summary>
        protected abstract MessageType ExpectedMessageType { get; }

        #region public modifier methods

        public void AddTag(ITag tag)
        {
            _tags.Add(tag);
        }
        public bool HasTag(ITag tag)
        {
            return _tags.Contains(tag);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().Name).Append(":").Append(MessageType).Append(" | ");
            builder.Append(TimeStamp).Append(" | ");
            builder.Append(Source).Append(" | ");
            return builder.ToString();
        }

        public bool Equals(BaseMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._tags, _tags) && 
                other._timestamp.Equals(_timestamp) && 
                Equals(other._source, _source) && 
                Equals(other._messageType, _messageType) && 
                Equals(other.CreatedBy, CreatedBy);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BaseMessage)) return false;
            return Equals((BaseMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_tags != null ? _tags.GetHashCode() : 0);
                result = (result*397) ^ _timestamp.GetHashCode();
                result = (result*397) ^ (_source != null ? _source.GetHashCode() : 0);
                result = (result*397) ^ _messageType.GetHashCode();
                result = (result*397) ^ (CreatedBy != null ? CreatedBy.GetHashCode() : 0);
                return result;
            }
        }

        #region implementation of IComparable
        public int CompareTo(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return -1;
            }

            BaseMessage other = (BaseMessage) obj;
            if ((this.Source == null) || other.Source == null) return 0; //cant compare, wtf?

            //otherwise use the RawMessage/Source to compare (which should compare on dates) 
            return (this.Source.CompareTo(other.Source));
        }
        #endregion

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
