using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using Ninject;
using NLog;
using Offr;
using Offr.CouchDB;
using Offr.Message;
using Offr.Repository;
using Offr.Services;

namespace OffrConsole
{
    class PushToCouchService : IBackgroundExceptionReceiver
    {
        public const string OFFERS_FILE = "data/messages.json";
        public const string INITIAL_TAGS_FILE = "data/initial_tags.json";
        public const string IGNORE_USER_FILE = "data/ignore_user_list.json";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public PushToCouchService()
        {
            Console.Out.WriteLine("Instantiating PushToCouchService");
        }

        public void Run() {

            IKernel kernel = Offr.Global.Kernel; //DI Container
            try
            {
                ITagRepository tagRepository = kernel.Get<ITagRepository>();
                if (tagRepository is IPersistedRepository)
                {
                    ((IPersistedRepository)tagRepository).FilePath = INITIAL_TAGS_FILE;
                    ((IPersistedRepository)tagRepository).InitializeFromFile();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorException("Failed during tag initialization", ex);
                Console.Out.WriteLine("Failure to load initial tags");
                throw;
            }

            try
            {
                IMessageRepository messageRepository = kernel.Get<IMessageRepository>();
                if (messageRepository is IPersistedRepository)
                {
                    ((IPersistedRepository)messageRepository).FilePath = OFFERS_FILE;
                    ((IPersistedRepository)messageRepository).InitializeFromFile();
                }
            }
            catch (Exception ex)
            {
                _log.ErrorException("Failed during message initialization from file", ex);
                Console.Out.WriteLine("Failure to load messages.json");
                throw;
            }

            //set up push end points
            string couchServer = ConfigurationManager.AppSettings["CouchServer"] ?? "http://chchneeds.org.nz/cdb"; //actually  won't work because it needs user/pass in the url
            string validDB =  ConfigurationManager.AppSettings["CouchDBValidOnly"] ?? "couchdb"; //actually  won't work because it needs user/pass in the url
            string allDB =  ConfigurationManager.AppSettings["CouchDBAll"] ?? "alldb"; //actually  won't work because it needs user/pass in the url
            ((PushToCouchDBReceiver)kernel.Get<IValidMessageReceiver>()).CouchServer = couchServer;
            ((PushToCouchDBReceiver)kernel.Get<IValidMessageReceiver>()).CouchDB = validDB;
            ((PushToCouchDBReceiver)kernel.Get<IAllMessageReceiver>()).CouchServer = couchServer;
            ((PushToCouchDBReceiver)kernel.Get<IAllMessageReceiver>()).CouchDB = allDB;

            PersistanceService.Start(this);
            RawMessagePollingService.Start(this);
            
            while (true)
            {
                Thread.Sleep(10000);
                Console.Out.Write(".");
            }
        }

        public void NotifyException(Exception ex)
        {
            LastException = ex;
        }

        public Exception LastException
        {
            get;
            private set;
        }
    }
}
