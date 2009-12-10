using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public interface IRawMessageReceiver
    {
        void Notify(IEnumerable<IRawMessage> updatedMessages);
        void Notify(IRawMessage updatedMessage);
    }
}
