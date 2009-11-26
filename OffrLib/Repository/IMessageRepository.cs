using System;
using System.Collections.Generic;
using System.Text;
using Offr.Common;
using Offr.Message;
using Offr.Repository;

namespace Offr
{
    public interface IMessageRepository: IMemCache
    {
        IMessage Get(string id);
        void Save(IMessage instance);
        void Remove(IMessage instance);
        IEnumerable<IMessage> AllMessages();
    }
}
