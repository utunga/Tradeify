using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Message
{
    public enum MessageType
    {
        offr,
        takedown,
        wanted,
        recipient_feedback,
        supplier_feedback,
        etc
    }
}
