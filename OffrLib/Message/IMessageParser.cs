using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Message
{
    public interface IMessageParser
    {
        IMessage Parse(IRawMessage source);
    }
}
