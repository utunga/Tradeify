using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Message
{
    public enum MessageType
    {
        offer,
        takedown,
        wanted,
        recipient_feedback,
        supplier_feedback,
        etc
    }
}
