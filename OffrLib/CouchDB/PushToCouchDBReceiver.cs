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
    public class PushToCouchDBReceiver : IValidMessageReceiver
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public string CouchServer { get; set; }

        public string CouchDB { get; set; }

        public PushToCouchDBReceiver()
        {
            CouchServer = ConfigurationManager.AppSettings["CouchServer"] ?? "http://chchneeds.couchone.com"; //actually this won't work because it needs user/pass in the url
            CouchDB = ConfigurationManager.AppSettings["CouchDB"] ?? "testing"; //unless set explicitly don't write to the db            
        }

        public void Push(IMessage validMessage )
        {
            string messageAsJSON = JSON.Serialize(validMessage);
            _log.Info("Pushing message " + validMessage.RawText + " to " + CouchServer + ":" + CouchDB);
            int networkAttempts = 3;
            while (networkAttempts>0) {
                try
                {
                    new DB().CreateDocument(CouchServer, CouchDB, messageAsJSON);
                }
                catch (WebException ex)
                {
                    networkAttempts--;
                    _log.ErrorException("Failure to push message to " + CouchServer + ":" + CouchDB + " will try again " + networkAttempts + " times.", ex);
                    Thread.Sleep(1000); //wait for one second
                }
            }
            //failed, but hopefully didn't bring the whole service down
        }
    }
}

