using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Demo
{
    public class MockRawMessage : IRawMessage
    {

        public IMessagePointer Pointer { get; set; }
        public IUserPointer CreatedBy { get; set; }
        public string Text { get; set; }
        public DateTime? EndBy { get; set; }
        public string EndByText { get;  set; }
        public DateTime Timestamp { get;  set; }
   

        //-- properties of the OfferMessage

        public string OfferText { get; set; }
        public string MoreInfoURL { get;  set; }
        public string Thumbnail { get; set; }
        public ILocation Location { get; set; }
        public IUserPointer OfferedBy { get; set; }
        
        internal readonly TagList _tags;
        public IList<ITag> Tags
        {
            get { return _tags; }
        }

        public MockRawMessage()
        {
            _tags = new TagList();
        }

        public MockRawMessage(int id)
        {
            Pointer = new TwitterMessagePointer(id);
            _tags = new TagList();
        }

        public MockRawMessage(string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, DateTime dateTimeUTC) : this()
        {
            Text = sourceText;
            Pointer = messagePointer;
            CreatedBy = createdBy;
            Timestamp = dateTimeUTC;
        }

        public MockRawMessage(string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, string dateTimeUTC) :
            this(sourceText, messagePointer, createdBy, DateUtils.UTCDateTimeFromTwitterTimeStamp(dateTimeUTC))
        {
        }

        public int CompareTo(IRawMessage otherIRawMessage)
        {
            if (otherIRawMessage is MockRawMessage)
            {
                MockRawMessage other = (MockRawMessage)otherIRawMessage;
                return this.Timestamp.CompareTo(other.Timestamp);
            }
            else
            {
                throw new NotSupportedException("Don't know how to compare a MockRawMessage and a " + otherIRawMessage.GetType());
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("#").Append(MessageType.offer).Append(" ");
            foreach (ITag tag in _tags.TagsOfType(TagType.group))
            {
                builder.Append("#").Append(tag.Text).Append(" ");
            }
            builder.Append(this.OfferText).Append(" ");
            if (Location != null)
            {
                builder.Append("in l:").Append(Location.Address).Append(": ");
            }
            builder.Append("for ");
            foreach (ITag tag in _tags.TagsOfType(TagType.currency))
            {
                builder.Append("#").Append(tag.Text).Append(" ");
            }
            builder.Append(MoreInfoURL).Append(" ");
            foreach (ITag tag in _tags.TagsOfType(TagType.tag))
            {
                builder.Append("#").Append(tag.Text).Append(" ");
            }
            if (EndByText != null)
            {
                builder.Append("[").Append(EndByText).Append("]");
            }
            return builder.ToString();
        }


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
            Pointer = JSON.ReadProperty<TwitterMessagePointer>(serializer, reader, "pointer");
            CreatedBy = JSON.ReadProperty<TwitterUserPointer>(serializer, reader, "created_by");
            Text = JSON.ReadProperty<string>(serializer, reader, "text");
            Timestamp = JSON.ReadProperty<DateTime>(serializer, reader, "timestamp_utc");
        }
        #endregion JSON
    }
}