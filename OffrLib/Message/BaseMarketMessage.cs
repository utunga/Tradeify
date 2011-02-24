using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public abstract class BaseMarketMessage : BaseMessage, IEquatable<BaseMarketMessage>
    {
        const decimal MAX_MESSAGE_LENGTH = 200;

        public string MessageText { get; set; }

        public ILocation Location { get; set; }
        
        public DateTime? EndBy { get; private set; }
        
        public string EndByText { get; private set; }
        
        public IUserPointer OfferedBy { get { return base.CreatedBy; } }

        public IEnumerable<ITag> Currencies
        {
            get { return _tags.TagsOfType(TagType.currency); }
        }

        public IEnumerable<ITag> LocationTags
        {
            get { return _tags.TagsOfType(TagType.loc); }
        }

        private readonly List<String> _thumbnails;
        
        public string Thumbnail
        {
            //return first thumb nail for now
            get
            {
                return (_thumbnails.Count >0) ? _thumbnails[0] : null;
            }
        }
      
        protected BaseMarketMessage()
            : base()
        {
            _thumbnails = new List<string>();
        }

        public override bool IsValid()
        {
            return ValidationFailReasons().Length == 0;
        }

        //useful for short circuiting the parsing process
        public override bool HasValidTags()
        {
            return (_tags.TagsOfType(TagType.group).Count > 0);
        }

        /// <summary>
        /// 
        /// to be a valid offer message it needs
        /// a currency (where 'free' can be a currency
        /// a location
        /// a group tag
        /// </summary>
        /// <returns></returns>
        public override string[] ValidationFailReasons()
        {
            var validationFails = new List<string>();
            if (MessageText.Length>MAX_MESSAGE_LENGTH)
            {
                validationFails.Add(ValidationFailReason.TooLong.ToString());
            }
            //OK not to have any currency tags
            //if (_tags.TagsOfType(TagType.currency).Count == 0)
            //{
            //    validationFails.Add(ValidationFailReason.NeedsCurrencyTag.ToString());
            //}
            if (_tags.TagsOfType(TagType.group).Count == 0) 
            {
                validationFails.Add(ValidationFailReason.NeedsGroupTag.ToString());
            }
            if (Location == null)
            {
                validationFails.Add(ValidationFailReason.NeedsLocation.ToString());
            }
            if (string.IsNullOrEmpty(MessageText))
            {
                validationFails.Add(ValidationFailReason.NeedsOfferMessage.ToString());
            }
            return validationFails.ToArray();
        }

        public void SetEndBy(string endByText, DateTime? endBy)
        {
            EndByText = endByText;
            EndBy = endBy;
        }

        public void ClearEndBy()
        {
            EndByText = null;
            EndBy = null;
        }

        public void AddThumbnail(string thumbnailURL)
        {
            if(thumbnailURL!=null)
                _thumbnails.Add(thumbnailURL);
        }

        public bool Equals(BaseMarketMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) &&
                   _thumbnails.SequenceEqual(other._thumbnails) &&
                   Equals(other.MessageText, MessageText)  &&
                   Equals(other.MoreInfoURL, MoreInfoURL) &&
                   other.EndBy.Equals(EndBy) &&
                   Equals(other.EndByText, EndByText) &&
                   Equals(other.Location, Location);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((OfferMessage) ((object) (obj as OfferMessage)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ (_thumbnails != null ? _thumbnails.GetHashCode() : 0);
                result = (result * 397) ^ (MessageText != null ? MessageText.GetHashCode() : 0);
                result = (result * 397) ^ (MoreInfoURL != null ? MoreInfoURL.GetHashCode() : 0);
                result = (result * 397) ^ (EndBy.HasValue ? EndBy.Value.GetHashCode() : 0);
                result = (result * 397) ^ (EndByText != null ? EndByText.GetHashCode() : 0);
                result = (result * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.ToString());
            builder.Append(":offer_text:").Append(MessageText);
            builder.Append(":profile_url:").Append(MoreInfoURL);
            builder.Append(":thumbnail:").Append(Thumbnail);
            builder.Append(":end_by:").Append(EndBy);
            builder.Append(":location:").Append(Location);
            return builder.ToString();
        }

        public override void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            base.ReadJson(reader, serializer);
            MessageText = JSON.ReadProperty<string>(serializer, reader, "offer_text");
            MoreInfoURL = JSON.ReadProperty<string>(serializer, reader, "more_info_url");
            AddThumbnail(JSON.ReadProperty<string>(serializer, reader, "thumbnail"));
            EndBy = JSON.ReadProperty<DateTime?>(serializer, reader, "end_by");
            EndByText = JSON.ReadProperty<string>(serializer, reader, "end_by_text");
            Location = JSON.ReadProperty<Location.Location>(serializer, reader, "location");
        }
        
        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            base.WriteJson(writer,serializer);
            JSON.WriteProperty(serializer, writer, "offer_text", MessageText);
            JSON.WriteProperty(serializer, writer, "more_info_url", MoreInfoURL);
            JSON.WriteProperty(serializer, writer, "thumbnail", Thumbnail);
            JSON.WriteProperty(serializer, writer, "end_by", EndBy);
            JSON.WriteProperty(serializer, writer, "end_by_text", EndByText);
            JSON.WriteProperty(serializer, writer, "location", Location);
        }

        public override bool IsExpired()
        {
            return EndBy != null && (EndBy.Value.CompareTo(DateTime.Now) <= -1);
        }

        
    }
}