using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;
using Ninject.Modules;
using Offr.Json;
using Offr.Message;
using Offr.OpenSocial;
using Offr.Repository;
using Offr.Text;

namespace Offr
{
    public static class Global
    {
        static readonly object[] _syncLock = new object[0];
        static IKernel _ninjectKernel;
      
        public static void Initialize(INinjectModule configuration)
        {
            lock (_syncLock)
            {
                if (_ninjectKernel == null)
                {
                    _ninjectKernel = new StandardKernel(configuration);
                }
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

        #region region of leaky abstractions - globals that should probably be Dependency injections or some thing?

        public static IRawMessageProvider GetRawMessageProvider()
        {
            return Kernel.Get<IRawMessageProvider>();
        }

        public static IMessageRepository GetMessageRepository()
        {
            return Kernel.Get<IMessageRepository>(); ;
        }

        public static ITagRepository GetTagRepository()
        {
            return Kernel.Get<ITagRepository>();
        }

        public static void NotifyRawMessage(IRawMessage message)
        {
            Kernel.Get<IRawMessageReceiver>().Notify(message);
        }

        #endregion

    }
}
