using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Offr.Location;
using Offr.Text;

namespace Offr.Message
{
    public class OfferMessage : BaseMessage, IOfferMessage
    {
        public static string HASHTAG = "#" + MessageType.offr_test;

        //FIXME: ideally the properties below would be internal set, not public
        public string OfferText { get; set; }
        public string MoreInfoURL { get; set; }
        public ILocation Location { get; set; }

        public IUserPointer OfferedBy { get { return base.CreatedBy; } }

        protected override MessageType ExpectedMessageType
        {
            get { return MessageType.offr_test; }
        }

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

    }
}
