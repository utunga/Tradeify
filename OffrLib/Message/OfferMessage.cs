﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public class OfferMessage : BaseMessage, IOfferMessage, IEquatable<OfferMessage>
    {
        #region fields
        public static string HASHTAG = "#" + MessageType.offr_test;

        public string OfferText { get; set; }

        public ILocation Location { get; set; }

        public IUserPointer OfferedBy { get { return base.CreatedBy; }
        }
        private List<String> _thumbnails;
        
        public DateTime? EndBy { get; private set; }

        public string EndByText { get; private set; }
        #endregion fields

        #region read only properties

        public IEnumerable<ITag> Currencies
        {
            get { return _tags.TagsOfType(TagType.type); }
        }

        public IEnumerable<ITag> LocationTags
        {
            get { return _tags.TagsOfType(TagType.loc); }
        }

        protected override MessageType ExpectedMessageType
        {
            get { return MessageType.offr_test; }
        }
       
        public string Thumbnail
        {
            //return first thumb nail for now
            get
            {
                return (_thumbnails.Count >0) ? _thumbnails[0] : null;
            }
        }

        #endregion

        public OfferMessage() : base()
        {
            _thumbnails = new List<string>();
        }

        #region overrides of abstract base class 
        
        public override bool IsValid()
        {
            // to be a valid offer message it needs
            // a type (of currency)
            // a location
            // a group tag
            if (_tags.TagsOfType(TagType.type).Count == 0)
            {
                return false;
            }
            if (Location==null)
            {
                return false;
            }
            if (_tags.TagsOfType(TagType.group).Count == 0) 
            {
                return false;
            }
            return true;
        }

        #endregion
        /// <summary>
        /// Set end by - both params must be supplied at same time.
        /// No attempt will be made to parse the 'end by' text
        /// FIXME: ideally these set methods would be internal, not public
        /// </summary>
        /// 
        /// 
        #region Setter Methods
        public void SetEndBy(string endByText, DateTime endBy)
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
        #endregion Setter Methods

        #region Equals
        public bool Equals(OfferMessage other)
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
            return Equals(obj as OfferMessage);
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

        #endregion Equals

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

        #region JSON
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
        #endregion
    }
}
