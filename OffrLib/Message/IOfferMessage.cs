using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Offr.Location;
using Offr.Message;
using Offr.Text;

namespace Offr.Message
{
    public interface IOfferMessage : IMessage
    {
        string OfferText { get; }
        string MoreInfoURL { get; }
        ILocation Location { get; }
        string EndByText { get; }
        DateTime? EndBy { get; }
        IUserPointer OfferedBy { get; }
        ReadOnlyCollection<ITag> Currencies { get; }
        ReadOnlyCollection<ITag> LocationTags { get; }
    }
}
