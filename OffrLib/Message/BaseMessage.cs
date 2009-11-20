using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Text;

namespace Offr.Message
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseMessage : IMessage, IEquatable<BaseMessage>,ICanJson
    {
        public TagList _tags;

        private MessageType _messageType;

        private IRawMessage _source;

        private DateTime _timestamp = DateTime.MinValue;

        [JsonProperty]
        public bool IsValid { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(UserPointerConverter))]
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
/*
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]*/
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
                Equals(other._source, _source) && 
                Equals(other._messageType, _messageType) && 
                Equals(other.CreatedBy, CreatedBy) &&
                other._timestamp.Equals(_timestamp);
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
        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            _tags.WriteJson(writer,serializer);

            JSON.WriteProperty(serializer, writer, "timestamp", _timestamp);

            JSON.WriteProperty(serializer, writer, "source", _source);

            JSON.WriteProperty(serializer, writer, "message_type", _messageType);

            JSON.WriteProperty(serializer, writer, "created_by", CreatedBy);
             
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            _tags = new TagList();
            _tags.ReadJson(reader,serializer);

            _timestamp = JSON.ReadProperty<DateTime>(serializer, reader, "timestamp");

            _source = JSON.ReadProperty<RawMessage>(serializer, reader, "source");

            _messageType = JSON.ReadProperty<MessageType>(serializer, reader, "message_type");

            CreatedBy = JSON.ReadProperty<TwitterUserPointer>(serializer, reader, "created_by");

        }
        #endregion JSON
    }
}
