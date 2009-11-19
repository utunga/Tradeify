using System;
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
    [JsonObject(MemberSerialization.OptIn)]
    public class OfferMessage : BaseMessage, IOfferMessage, IEquatable<OfferMessage>
    {
        public static string HASHTAG = "#" + MessageType.offr_test;

        [JsonProperty]
        public string OfferText { get; set; }


        [JsonProperty]
        [JsonConverter(typeof(LocationConverter))]
        public ILocation Location { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(UserPointerConverter))]
        public IUserPointer OfferedBy { get { return base.CreatedBy; }
            /*private set { base.CreatedBy = value; */
        }

        [JsonProperty]
        private List<String> _thumbnails;
        
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? EndBy { get; private set; }

        [JsonProperty]
        public string EndByText { get; private set; }

        #region read only properties

        public ReadOnlyCollection<ITag> Currencies
        {
            get { return _tags.TagsOfType(TagType.type); }
        }

        public ReadOnlyCollection<ITag> LocationTags
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

        /// <summary>
        /// Set end by - both params must be supplied at same time.
        /// No attempt will be made to parse the 'end by' text
        /// FIXME: ideally these set methods would be internal, not public
        /// </summary>
        /// 
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
            _thumbnails.Add(thumbnailURL);
        }

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
        #region JSON
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            base.WriteJson(writer,serializer);
            /*Equals(other._thumbnails, _thumbnails) &&
            Equals(other.OfferText, OfferText) &&
            Equals(other.MoreInfoURL, MoreInfoURL) &&
            other.EndBy.Equals(EndBy) &&
            Equals(other.EndByText, EndByText) &&
            Equals(other.Location, Location);*/
            /*serializer.Serialize(writer,Thumbnail);
            serializer.Serialize(writer,OfferText);
            serializer.Serialize(writer,MoreInfoURL);
            serializer.Serialize(writer,EndBy);
            serializer.Serialize(writer,EndByText);
            serializer.Serialize(writer,EndByText);
            serializer.Serialize(writer,Location);*/
            JSON.WriteProperty(serializer, writer, "Thumbnail", Thumbnail);
            JSON.WriteProperty(serializer, writer, "OfferText", OfferText);
            JSON.WriteProperty(serializer, writer, "MoreInfoURL", MoreInfoURL);
            JSON.WriteProperty(serializer, writer, "EndBy", EndBy);
            JSON.WriteProperty(serializer, writer, "EndByText", EndByText);
            JSON.WriteProperty(serializer, writer, "Location", Location);

        }
        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            base.ReadJson(reader,serializer);
/*            serializer.Deserialize(reader, typeof (string));
            serializer.Deserialize(reader, typeof (string));
            serializer.Deserialize(reader, typeof (string));
            serializer.Deserialize(reader, typeof (DateTime?));
            serializer.Deserialize(reader, typeof (string));
            serializer.Deserialize(reader, typeof (string));
            serializer.Deserialize(reader, typeof (Location.Location));*/
            AddThumbnail(JSON.ReadProperty<string>(serializer, reader, "Thumbnail"));

            OfferText = JSON.ReadProperty<string>(serializer, reader, "OfferText");

            MoreInfoURL = JSON.ReadProperty<string>(serializer, reader, "MoreInfoURL");

            EndBy = JSON.ReadProperty<DateTime?>(serializer, reader, "EndBy");

            EndByText = JSON.ReadProperty<string>(serializer, reader, "EndByText");

            Location = JSON.ReadProperty<Location.Location>(serializer, reader, "Location");
           

        }
        #endregion
    }
}
