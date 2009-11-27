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
    public static class TwitterPollingService
    {
        public static readonly int POLLING_INTERVAL =ConfigurationManager.AppSettings["PersistenceService_PollingInterval"] == null ?
                    30000 ://30 seconds ? 
                    int.Parse(ConfigurationManager.AppSettings["PersistenceService_PollingInterval"]);
        private static readonly object[] _syncLock;
        private static IRawMessageProvider _provider;
        private static IBackgroundExceptionReceiver _exceptionReceiver;

        static TwitterPollingService()
        {
            _syncLock = new object[0];
            _provider = Global.Kernel.Get<IRawMessageProvider>();

        }
        private static void Start(IBackgroundExceptionReceiver exceptionReceiver)
        {
            lock (_syncLock)
            {
                _exceptionReceiver = exceptionReceiver;
                Thread thread = new Thread(Run);
                thread.IsBackground = true; //NOTE2J -- very important this one
                thread.Start();
            }

        }
        public static void EnsureStarted(IBackgroundExceptionReceiver exceptionReceiver)
        {
            //hopefully 99.999% of the time we return straight away..
            //_log.Error("PersistanceService not started, will force it to start");
            lock (_syncLock)
            {
                Start(exceptionReceiver);
            }
        }
        static void Run()
        {
            // long lastUpdate = 0;
            while (true) //continue till the end of time or until the thread dies
            {
                _provider.Update();
                Thread.Sleep(POLLING_INTERVAL);
            }

        }
    }
}
