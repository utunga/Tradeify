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
        string MessageText { get; }
        string MoreInfoURL { get; }
        string Thumbnail { get; }
        ILocation Location { get; }
        string EndByText { get; }
        DateTime? EndBy { get; }
        IUserPointer OfferedBy { get; }
        IEnumerable<ITag> Currencies { get; }
        IEnumerable<ITag> LocationTags { get; }
    }
}
