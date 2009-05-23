using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;

namespace Offr
{
    public static class Global
    {
        static readonly object[] _syncLock = new object[0];
        static IKernel _ninjectKernel;
      
        public static void Initialize(IModule configuration)
        {
            lock (_syncLock)
            {
                if (_ninjectKernel != null)
                {
                    throw new ApplicationException("Kernel already initialized");
                }
                _ninjectKernel = new StandardKernel(configuration);
            }
        }

        public static IKernel Kernel
        {
            get 
            {
                if (_ninjectKernel == null)
                {
                    InitializeDefaultConfig();
                }
                return _ninjectKernel;
            }
        }

        private static void InitializeDefaultConfig()
        {
            Initialize(new DefaultNinjectConfig());
        }
    }
}
