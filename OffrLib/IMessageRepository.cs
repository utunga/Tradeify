using System;
using System.Collections.Generic;
using System.Text;
using Offr.Message;

namespace Offr
{
    public interface IMessageRepository
    {
        IMessage Get(string id);
        void Save(IMessage instance);
        void Remove(IMessage instance);
    }
}
