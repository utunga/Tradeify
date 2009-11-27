using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Repository
{
    public interface IPersistedRepository
    {
        bool IsDirty { get; }
        string FilePath { get; set; }
        void InitializeFromFile();
        void SerializeToFile();
        
    }
}
