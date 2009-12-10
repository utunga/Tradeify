using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public interface IRawMessageReceiver
    {
        void Notify(IEnumerable<IRawMessage> messages);
        void Notify(IRawMessage messages);
    }
}
