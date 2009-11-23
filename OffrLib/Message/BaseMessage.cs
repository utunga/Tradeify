using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Text;

namespace Offr.Message
{

    public abstract class BaseMessage : IMessage, IEquatable<BaseMessage>,ICanJson
    {
        public TagList _tags;

        private MessageType _messageType;

        public bool IsValid { get; set; }

        public IUserPointer CreatedBy { get; set; }

        public string MoreInfoURL { get; set; }

        public IEnumerable<ITag> Tags
        {
            get { return _tags; }
            private set { _tags = new TagList(value); }
        }

        public MessageType MessageType
        {
            get { return _messageType; }
            internal set
            {
                _messageType = value;
            }
        }

        public IMessagePointer MessagePointer
        {
            get ; set; 
        }

        public string RawText
        {
            get; set; 
        }

        public string Timestamp
        {
            get; set;
        }

        #region read only properties
        
        public string ID
        {
            get
            {
                 if (MessagePointer != null) return MessagePointer.MatchTag;
                else return null;
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
            builder.Append(Timestamp).Append(" | ");
           // builder.Append(Source).Append(" | ");
            return builder.ToString();
        }

        public bool Equals(BaseMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._tags, _tags) && 
                Equals(other._messageType, _messageType)    && 
                /*other.IsValid.Equals(IsValid)               && */
                Equals(other.CreatedBy,CreatedBy)          && 
                Equals(other.MoreInfoURL, MoreInfoURL)      && 
                Equals(other.MessagePointer, MessagePointer)&& 
                Equals(other.RawText, RawText)              && 
                Equals(other.Timestamp, Timestamp);
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
                result = (result*397) ^ _messageType.GetHashCode();
                result = (result*397) ^ IsValid.GetHashCode();
                result = (result*397) ^ (CreatedBy != null ? CreatedBy.GetHashCode() : 0);
                result = (result*397) ^ (MoreInfoURL != null ? MoreInfoURL.GetHashCode() : 0);
                result = (result*397) ^ (MessagePointer != null ? MessagePointer.GetHashCode() : 0);
                result = (result*397) ^ (RawText != null ? RawText.GetHashCode() : 0);
                result = (result*397) ^ (Timestamp != null ? Timestamp.GetHashCode() : 0);
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
            return this.Timestamp.CompareTo(other.Timestamp);
            //if ((this.Source == null) || other.Source == null) return 0; //cant compare, wtf?

            //otherwise use the RawMessage/Source to compare (which should compare on dates) 
            //return (this.Source.CompareTo(other.Source));
        }
        #endregion
        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "message_type", _messageType.ToString());
            JSON.WriteProperty(serializer, writer, "timestamp", Timestamp);

            _tags.WriteJson(writer,serializer);

            JSON.WriteProperty(serializer,writer,"raw_text",RawText);	
            JSON.WriteProperty(serializer,writer,"message_pointer",MessagePointer);
           

            //JSON.WriteProperty(serializer, writer, "source", _source);

            JSON.WriteProperty(serializer, writer, "created_by", CreatedBy);
             
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer){
 
            _messageType = JSON.ReadProperty<MessageType>(serializer, reader, "message_type");
            Timestamp=JSON.ReadProperty<string>(serializer, reader, "timestamp");

            _tags = new TagList(); 
            _tags.ReadJson(reader, serializer);

            //_source = JSON.ReadProperty<RawMessage>(serializer, reader, "source");
            RawText = JSON.ReadProperty<string>(serializer, reader, "raw_text");
            MessagePointer =JSON.ReadProperty<IMessagePointer>(serializer,reader,"message_pointer")	;


           
            CreatedBy = JSON.ReadProperty<TwitterUserPointer>(serializer, reader, "created_by");

        }
        #endregion JSON
    }
}
