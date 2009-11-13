﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Ninject.Core;
using Offr.Json;
using Offr.Message;

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
                //if (_ninjectKernel == null)
                //{
                    _ninjectKernel = new StandardKernel(configuration);
                    //FIXME log that it was already initialized
                //}
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
