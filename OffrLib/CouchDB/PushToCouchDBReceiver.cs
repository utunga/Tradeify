using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NLog;
using Offr.Json;
using Offr.Message;

namespace Offr.CouchDB
{
    public class PushToCouchDBReceiver : IValidMessageReceiver, IAllMessageReceiver
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public string CouchServer { get; set; }
        public string CouchDB { get; set; }

        public PushToCouchDBReceiver()
        {          
        }

        public void Push(IMessage message)
        {
            string messageAsJSON = JSON.Serialize(message);
            _log.Info("Pushing message to |" + CouchDB + "| '" + message.RawText + "' /" + CouchServer  );
            int networkAttempts = 5;
            while (--networkAttempts>0) {
                try
                {
                    new DB().CreateDocument(CouchServer, CouchDB, messageAsJSON);
                }
                catch (WebException ex)
                {
                    
                    _log.ErrorException("Failure to push message to " + CouchServer + ":" + CouchDB + " will try again " + networkAttempts + " times.", ex);
                    Thread.Sleep(500); //wait for half a second
                }
            }
            //failed, but hopefully didn't bring the whole service down
        }
    }
}

