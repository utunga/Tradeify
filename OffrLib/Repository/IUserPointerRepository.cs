using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Repository
{
    public interface IUserPointerRepository
    {
        IUserPointer Get(string id);
        void Save(IUserPointer instance);
        void Remove(IUserPointer instance);
    }
}
