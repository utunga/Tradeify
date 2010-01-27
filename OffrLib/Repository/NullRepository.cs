using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Repository
{
    public class NullRepository<T> 
    {
        bool IsDirty { get { return false; } }
        int Count { get { return 0; } }
        IQueryable<T> GetAll() { return (new List<T>()).AsQueryable(); }
        T Get(string id) { return default(T); }
        void Save(T instance) { }
        void Remove(T instance) { }
    }

}
