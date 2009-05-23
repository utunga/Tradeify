﻿using System;
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

        public string OfferText { get; internal set; }
        public string MoreInfoURL { get; internal set; }
        public ILocation Location { get; internal set; }

        public IUserPointer OfferedBy { get { return base.CreatedBy; } }

        protected override MessageType ExpectedMessageType
        {
            get { return MessageType.offr_test; }
        }

        public ReadOnlyCollection<ITag> Currencies
        {
            get { return _tags.TagsOfType(TagType.currency); }
        }

        public ReadOnlyCollection<ITag> LocationTags
        {
            get { return _tags.TagsOfType(TagType.location); }
        }

        public DateTime? EndBy { get; private set; }
        public string EndByText { get; private set; }

        /// <summary>
        /// Set end by - both params must be supplied at same time.
        /// No attempt will be made to parse the 'end by' text
        /// </summary>
        internal void SetEndBy(string endByText, DateTime endBy)
        {
            EndByText = endByText;
            EndBy = endBy;
        }

        internal void ClearEndBy()
        {
            EndByText = null;
            EndBy = null;
        }

    }
}
