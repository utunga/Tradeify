using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Offr.Repository;
using Offr.Text;

namespace Offr.Services
{
    public static class RawMessagePollingService
    {
        public static readonly int POLLING_INTERVAL =ConfigurationManager.AppSettings["RawMessagePollingService_PollingInterval"] == null ?
                    1000 ://1 second
                    int.Parse(ConfigurationManager.AppSettings["RawMessagePollingService_PollingInterval"]);
       
        private static readonly object[] _syncLock;
        private static IRawMessageProvider _provider;
        private static IBackgroundExceptionReceiver _exceptionReceiver;

        static RawMessagePollingService()
        {
            _syncLock = new object[0];
            //FIXME this will need to change - also makes it hard to test
            _provider = Global.Kernel.Get<IRawMessageProvider>();
        }

        public static void Start(IBackgroundExceptionReceiver exceptionReceiver)
        {
            lock (_syncLock)
            {
                _exceptionReceiver = exceptionReceiver;
                Thread thread = new Thread(Run);
                thread.IsBackground = true; //NOTE2J -- very important this one
                thread.Start();
            }
        }

        //public static void EnsureStarted(IBackgroundExceptionReceiver exceptionReceiver)
        //{
        //    //hopefully 99.999% of the time we return straight away..
        //    if (_busy) return;
        //    lock (_syncLock)
        //    {
        //        Start(exceptionReceiver);
        //    }
        //}

        static void Run()
        {
            // long lastUpdate = 0;
            while (true) //continue till the end of time or until the thread dies
            {
                try
                {
                    _provider.Update();
                     Thread.Sleep(POLLING_INTERVAL);
                }
                catch (Exception ex)
                {
                    _exceptionReceiver.NotifyException(ex);
                }
            }
        }
    }
}
