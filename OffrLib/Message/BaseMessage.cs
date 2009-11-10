﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public abstract class BaseMessage : IMessage
    {
        //FIXME ideally many of the 'set' methods below would be internal, not public
        public bool IsValid { get; set; }
        public IUserPointer CreatedBy { get; set; }
        
        internal readonly TagList _tags;
        public IList<ITag> Tags
        {
            get { return _tags; }
        }
        
        public bool HasTag(ITag tag)
        {
            return _tags.Contains(tag);
        }

        DateTime _timestamp = DateTime.MinValue;
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            private set { _timestamp = value;}
        }
       
        IRawMessage _source;
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
                if (value != ExpectedMessageType)
                {
                    throw new MessageParseException("Cannot set message type to " + value + " on a " + GetType().Name + ". Expected " + ExpectedMessageType);
                }
                _messageType = value;
            }
        }


        /// <summary>
        /// Implementing classes should supply The MessageType they expect for their class and the base will throw a parse expection if it doesn't match
        /// </summary>
        protected abstract MessageType ExpectedMessageType { get; }

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

        public string ID
        {
            get { throw new NotImplementedException(); }
        }
    }
}
