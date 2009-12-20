using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Twitter;

namespace Offr.Text
{
    public class RawMessage : IRawMessage , IEquatable<RawMessage>
    {
        public IMessagePointer Pointer { get; protected set; }

        public IUserPointer CreatedBy { get; protected set; }

        public string Text { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        internal RawMessage()
        {
        }

        public bool Equals(RawMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return  Equals(other.Pointer, Pointer) && 
                    Equals(other.CreatedBy, CreatedBy) && 
                    Equals(other.Text, Text) && 
                    other.Timestamp.Equals(Timestamp);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RawMessage)) return false;
            return Equals((RawMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Pointer != null ? Pointer.GetHashCode() : 0);
                result = (result*397) ^ (CreatedBy != null ? CreatedBy.GetHashCode() : 0);
                result = (result*397) ^ (Text != null ? Text.GetHashCode() : 0);
                result = (result*397) ^ Timestamp.GetHashCode();
                return result;
            }
        }

        #region Implementation of IComparable<IRawMessage>

        public int CompareTo(IRawMessage otherIRawMessage)
        {
            if (otherIRawMessage is RawMessage)
            {
                RawMessage other = (RawMessage) otherIRawMessage;
                return this.Timestamp.CompareTo(other.Timestamp);
            }
            else
            {
                throw new NotSupportedException("Don't know how to compare a RawMessage and a " + otherIRawMessage.GetType() );
            }
        }

        #endregion

        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "pointer", Pointer);
            JSON.WriteProperty(serializer, writer, "created_by", CreatedBy);
            JSON.WriteProperty(serializer, writer, "text", Text);
            JSON.WriteProperty(serializer, writer, "timestamp_utc", Timestamp);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            Pointer=JSON.ReadProperty<TwitterMessagePointer>(serializer, reader, "pointer");
            CreatedBy=JSON.ReadProperty<TwitterUserPointer>(serializer, reader, "created_by");
            Text=JSON.ReadProperty<string>(serializer, reader, "text");
            Timestamp = JSON.ReadProperty<DateTime>(serializer, reader, "timestamp_utc");
        }
        #endregion JSON


    }

 
}
