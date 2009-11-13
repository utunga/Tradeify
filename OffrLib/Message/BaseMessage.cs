using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Text;

namespace Offr.Message
{
    public abstract class BaseMessage : IMessage
    {
        //FIXME ideally many of the 'set' methods below would be internal, not public
        public bool IsValid { get; set; }
        [JsonConverter(typeof(IUserPointerConverter))]
        public IUserPointer CreatedBy { get; set; }

        public string ID
        {
            get { if (_source != null) if (_source.Pointer != null) return _source.Pointer.MatchTag; 
            return null;
            }
        }

        internal readonly TagList _tags;

        public IList<ITag> Tags
        {
            get { return _tags; }
        }

        DateTime _timestamp = DateTime.MinValue;
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            private set { _timestamp = value;}
        }
       
       
        IRawMessage _source;
        [JsonConverter(typeof(RawMessageConverter))]
        public IRawMessage Source { 
            get 
            {
                return _source;
            }
            set
            {
                _source = value;
                _timestamp = DateTime.MinValue; 
                DateTime.TryParse(_source.Timestamp, out _timestamp);
            }
        }

        //[OnSerializing]
        //internal void OnSerializingMethod(StreamingContext context)
        //{
        //    Source = new RawMessage();
        //}

        public ReadOnlyCollection<ITag> HashTags
        {
            get { return _tags.TagsOfType(TagType.tag); }
        }

        public ReadOnlyCollection<ITag> CommunityTags
        {
            get { return _tags.TagsOfType(TagType.group); }
        }

        private MessageType _messageType;
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

        protected BaseMessage()
        {
            _tags = new TagList();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().Name).Append(":").Append(MessageType).Append(" | ");
            builder.Append(TimeStamp).Append(" | ");
            builder.Append(Source).Append(" | ");
            return builder.ToString();
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

     
    }
}
