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

        private List<String> _thumbnails;
        public BaseMarketMessage() : base()
        {
            _thumbnails = new List<string>();
        }

        public string OfferText { get; set; }
        public ILocation Location { get; set; }

        public IUserPointer OfferedBy { get { return base.CreatedBy; }
        }

        public DateTime? EndBy { get; private set; }
        public string EndByText { get; private set; }

        public IEnumerable<ITag> Currencies
        {
            get { return _tags.TagsOfType(TagType.type); }
        }

        public IEnumerable<ITag> LocationTags
        {
            get { return _tags.TagsOfType(TagType.loc); }
        }

        public string Thumbnail
        {
            //return first thumb nail for now
            get
            {
                return (_thumbnails.Count >0) ? _thumbnails[0] : null;
            }
        }

        public override bool IsValid()
        {
            return ValidationFailReasons().Length == 0;
        }

        /// <summary>
        /// 
        /// to be a valid offer message it needs
        /// a type (of currency)
        /// a location
        /// a group tag
        /// </summary>
        /// <returns></returns>
        public override string[] ValidationFailReasons()
        {
            var validationFails = new List<string>();
            if (_tags.TagsOfType(TagType.type).Count == 0)
            {
                validationFails.Add(ValidationFailReason.NeedsCurrencyTag.ToString());
            }
            if (Location==null)
            {
                validationFails.Add(ValidationFailReason.NeedsLocation.ToString());
            }
            if (_tags.TagsOfType(TagType.group).Count == 0) 
            {
                validationFails.Add(ValidationFailReason.NeedsGroupTag.ToString());
            }
            if (string.IsNullOrEmpty(OfferText))
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
                   Equals(other.OfferText, OfferText)  &&
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
                result = (result * 397) ^ (OfferText != null ? OfferText.GetHashCode() : 0);
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
            builder.Append(":offer_text:").Append(OfferText);
            builder.Append(":more_info_url:").Append(MoreInfoURL);
            builder.Append(":thumbnail:").Append(Thumbnail);
            builder.Append(":end_by:").Append(EndBy);
            builder.Append(":location:").Append(Location);
            return builder.ToString();
        }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            base.WriteJson(writer,serializer);
            JSON.WriteProperty(serializer, writer, "offer_text", OfferText);
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

        public override void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            base.ReadJson(reader,serializer);
            OfferText = JSON.ReadProperty<string>(serializer, reader, "offer_text");
            MoreInfoURL = JSON.ReadProperty<string>(serializer, reader, "more_info_url");
            AddThumbnail(JSON.ReadProperty<string>(serializer, reader, "thumbnail")); 
            EndBy = JSON.ReadProperty<DateTime?>(serializer, reader, "end_by");
            EndByText = JSON.ReadProperty<string>(serializer, reader, "end_by_text");
            Location = JSON.ReadProperty<Location.Location>(serializer, reader, "location");
        }
    }
}