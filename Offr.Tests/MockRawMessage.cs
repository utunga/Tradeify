using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Location;
using Offr.Message;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    public class MockRawMessage : IRawMessage
    {
        public IMessagePointer Pointer { get; internal set; }
        public IUserPointer CreatedBy { get; internal set; }
        public string Text { get; internal set; }
        public DateTime? EndBy { get; internal set; }
        public string EndByText { get; internal set; }

        public DateTime Timestamp
        {
            get; 
            internal set;
        }

        //-- properties of the OfferMessage

        public string OfferText { get; internal set; }
        public string MoreInfoURL { get; internal set; }
        public string Thumbnail { get; internal set; }
        public ILocation Location { get; internal set; }
        public IUserPointer OfferedBy { get; internal set; }
        
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
            Pointer = new MockMessagePointer(id);
            _tags = new TagList();
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
            builder.Append("#").Append(MessageType.offr_test).Append(" ");
            foreach (ITag tag in _tags.TagsOfType(TagType.group))
            {
                builder.Append("#").Append(tag.tag).Append(" ");
            }
            builder.Append(this.OfferText).Append(" ");
            builder.Append("in l:").Append(Location.Address).Append(": ");
            builder.Append("for ");
            foreach (ITag tag in _tags.TagsOfType(TagType.type))
            {
                builder.Append("#").Append(tag.tag).Append(" ");
            }
            builder.Append(MoreInfoURL).Append(" ");
            foreach (ITag tag in _tags.TagsOfType(TagType.tag))
            {
                builder.Append("#").Append(tag.tag).Append(" ");
            }
            if (EndByText != null)
            {
                builder.Append("[").Append(EndByText).Append("]");
            }
            return builder.ToString();
        }

        #region Implementation of ICanJson

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            //So writing isnt screwed up
            RawMessage nonMock= new RawMessage(this.Text, this.Pointer, this.CreatedBy, this.Timestamp);
            nonMock.WriteJson(writer,serializer);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            Pointer = JSON.ReadProperty<TwitterMessagePointer>(serializer, reader, "Pointer");
        }
        #endregion
    }
}