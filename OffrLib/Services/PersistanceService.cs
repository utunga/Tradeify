using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Offr.Repository;

namespace Offr.Services
{
    /// <summary>
    /// Responsible for making sure that we persist relevant repository data to file every so often - poor mans db.
    /// </summary>
    public static class PersistanceService
    {
         // how often to 'wake up' and check if we need to save data, in milliseconds
         public static readonly int POLLING_INTERVAL =
            ConfigurationManager.AppSettings["PersistenceService_PollingInterval"] == null ?
            1000 : // 1 second
            int.Parse(ConfigurationManager.AppSettings["PersistenceService_PollingInterval"]);

        #region private properties

        //NOTE2J we need a logger class here
        //private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly object[] _syncLock; // NOTE2J note the use of this '_syncLock' pattern
        private static bool _busy;
        private static bool _stopped;
        private static IBackgroundExceptionReceiver _exceptionReceiver;
        private static readonly List<IPersistedRepository> _repositories;

        public static bool IsBusy
        {
            get { return _busy; }
        }

        #endregion

        #region static constructor and startup

        /// <summary>
        /// configguration in code - list of repositories that are worth persisting
        /// </summary>
        private static IEnumerable<IPersistedRepository> RepositoriesToPersist
        {
            get
            {
                var messages = Global.GetMessageRepository();
                if (messages is IPersistedRepository)
                    yield return messages as IPersistedRepository;
            }
        }

        static PersistanceService()
        {
            _syncLock = new object[0];
            _busy = false;
            _stopped = false;
            _repositories = new List<IPersistedRepository>(RepositoriesToPersist);

        }

        public static void Start(IBackgroundExceptionReceiver exceptionReceiver)
        {
                lock (_syncLock)
                {
                    _exceptionReceiver = exceptionReceiver;
                    _stopped = false;
                    Thread thread = new Thread(Run);
                    thread.IsBackground = true; //NOTE2J -- very important this one
                    thread.Start();
                }
            
        }

        public static void Stop()
        {
            _stopped = true;
            //wait while still busy
            while (_busy);
        }

        #endregion

        #region run method (and sleep intervals etc)

        static void Run()
        {
           // long lastUpdate = 0;
            try
            {
                while (!_stopped) //continue till the end of time or until the thread dies
                {
                    _busy = true;
                  
                    EnsurePersisted();
                    
                    Thread.Sleep(POLLING_INTERVAL);
                }
            }
            finally
            {
                // if we get to here something blew up
                //_log.Error("RepositoryService ended but it should never end");
                _busy = false;
            }
        }

        #endregion

        //-------------------------------------
        //    METHOD THAT DOES THE ACTUAL WORK
        //-------------------------------------

        private static void EnsurePersisted()
        {
            foreach (IPersistedRepository repository in _repositories)
            {
                try
                {
                    if (repository.IsDirty)
                    {
                        repository.SerializeToFile();
                    }
                }
                catch (Exception ex)
                {
                    // _log.Error("Failure to save data for "+ repository, ex);

                    if (_exceptionReceiver != null)
                    {
                        _exceptionReceiver.NotifyException(new ApplicationException("Failure during attempt to serialize " + repository, ex));
                    }
                }
            }
        }

    }
}
