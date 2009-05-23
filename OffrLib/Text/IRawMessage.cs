using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;

namespace Offr.Text
{
    public interface IRawMessage
    {
        IMessagePointer Pointer { get; }
        IUserPointer CreatedBy { get; }
        string Text { get; }
        string Timestamp { get; }
    }
}
