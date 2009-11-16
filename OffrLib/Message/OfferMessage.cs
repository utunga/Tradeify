using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json.Converter;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public class OfferMessage : BaseMessage, IOfferMessage, IEquatable<OfferMessage>
    {
        public static string HASHTAG = "#" + MessageType.offr_test;

        public string OfferText { get; set; }

        public string MoreInfoURL { get; set; }

        private List<String> _Thumbnails= new List<string>();
   
        public DateTime? EndBy { get; private set; }

        public string EndByText { get; private set; }

        [JsonConverter(typeof(ILocationConverter))]
        public ILocation Location { get; set; }

        [JsonConverter(typeof(IUserPointerConverter))]
        public IUserPointer OfferedBy { get { return base.CreatedBy; } }

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

        public String Thumbnail
        {
            //return first thumb nail for now
            get{return _Thumbnails[0]; }
        }
        /// <summary>
        /// Set end by - both params must be supplied at same time.
        /// No attempt will be made to parse the 'end by' text
        /// FIXME: ideally these set methods would be internal, not public
        /// </summary>
        public void SetEndBy(string endByText, DateTime endBy)
        {
            EndByText = endByText;
            EndBy = endBy;
        }

        public void addThumbnail(String s)
        {
            _Thumbnails.Add(s);
            //Thumbnail = s;
        }

        public void ClearEndBy()
        {
            EndByText = null;
            EndBy = null;
        }

        public bool Equals(OfferMessage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) &&
                Equals(other._Thumbnails, _Thumbnails) &&
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
                result = (result * 397) ^ (_Thumbnails != null ? _Thumbnails.GetHashCode() : 0);
                result = (result * 397) ^ (OfferText != null ? OfferText.GetHashCode() : 0);
                result = (result * 397) ^ (MoreInfoURL != null ? MoreInfoURL.GetHashCode() : 0);
                result = (result * 397) ^ (EndBy.HasValue ? EndBy.Value.GetHashCode() : 0);
                result = (result * 397) ^ (EndByText != null ? EndByText.GetHashCode() : 0);
                result = (result * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                return result;
            }
        }
    }
}
