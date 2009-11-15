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
        protected override MessageType ExpectedMessageType
        {
            get { return MessageType.offr_test; }
        }
        public static string HASHTAG = "#" + MessageType.offr_test;
        public string OfferText { get; set; }
        public string MoreInfoURL { get; set; }
        [JsonConverter(typeof(ILocationConverter))] public ILocation Location { get; set; }
        [JsonConverter(typeof(IUserPointerConverter))] public IUserPointer OfferedBy { get { return base.CreatedBy; } }      
        public ReadOnlyCollection<ITag> Currencies
        {
            get { return _tags.TagsOfType(TagType.type); }
        }
        public ReadOnlyCollection<ITag> LocationTags
        {
            get { return _tags.TagsOfType(TagType.loc); }
        }
        public DateTime? EndBy { get; private set; }
        public string EndByText { get; private set; }

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

        public void ClearEndBy()
        {
            EndByText = null;
            EndBy = null;
        }

        public bool Equals(OfferMessage other)
        {
            if (other != null)
            {
                if(this==other) return true;
                return Equals(OfferText, other.OfferText) &&
                       Equals(Location,other.Location) &&
                       Equals(EndBy,other.EndBy) &&
                       Equals(EndByText,other.EndByText) &&
                       Equals(Currencies,other.Currencies) &&
                       Equals(MoreInfoURL,other.MoreInfoURL) &&
                       Equals(LocationTags, other.LocationTags) &&
                       Equals(OfferedBy,other.OfferedBy) &&
                       Equals(ExpectedMessageType,other.ExpectedMessageType);
            }
            else return false;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int result = (OfferText != null ? OfferText.GetHashCode() : 0);
                result = (result*397) ^ (MoreInfoURL != null ? MoreInfoURL.GetHashCode() : 0);
                result = (result*397) ^ (Location != null ? Location.GetHashCode() : 0);
                result = (result*397) ^ (EndBy.HasValue ? EndBy.Value.GetHashCode() : 0);
                result = (result*397) ^ (EndByText != null ? EndByText.GetHashCode() : 0);
                result = (result * 397) ^ (LocationTags != null ? LocationTags.GetHashCode() : 0);
                return result;
            }
        }
    }
}
