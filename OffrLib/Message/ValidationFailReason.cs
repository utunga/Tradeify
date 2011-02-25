using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Message
{
    public enum ValidationFailReason
    {
        NeedsCurrencyTag, NeedsLocation, NotInNewZealand, NeedsGroupTag, NeedsOfferMessage, TooLong
    }
}
