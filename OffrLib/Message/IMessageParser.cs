using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Repository;
using Offr.Text;

namespace Offr.Message
{
    public interface IMessageParser
    {
        ITagRepository TagProvider { get; }
        IMessage Parse(IRawMessage source);
    }
}
