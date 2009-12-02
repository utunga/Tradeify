using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Ninject.Core;
using NLog;
using Offr;
using Offr.Message;
using Offr.Repository;
using Offr.Services;

namespace twademe
{
    public class Global : System.Web.HttpApplication, IBackgroundExceptionReceiver
    {
        public const string INITIAL_OFFERS_FILE = "/data/initial_offers.json";
        public const string INITIAL_TAGS_FILE = "/data/initial_tags.json";
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static bool IsProductionDeployment
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["IsProductionDeployment"]); }
        }
        public static bool HaltOnBackgroundExceptionsInDev
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["HaltOnBackgroundExceptionsInDev"]); }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                IMessageRepository messageRepository = Kernel.Get<IMessageRepository>();
                if (messageRepository is IPersistedRepository)
                {
                    ((IPersistedRepository) messageRepository).FilePath = Server.MapPath(INITIAL_OFFERS_FILE);
                    ((IPersistedRepository) messageRepository).InitializeFromFile();
                }
            }
            catch (Exception ex)
            {
                NotifyException(new ApplicationException("Failed during message initialization",ex));
            }

            try
            {
                ITagRepository tagRepository = Kernel.Get<ITagRepository>();
                if (tagRepository is IPersistedRepository)
                {
                    ((IPersistedRepository) tagRepository).FilePath = Server.MapPath(INITIAL_TAGS_FILE);
                    ((IPersistedRepository) tagRepository).InitializeFromFile();
                }
            }
            catch (Exception ex)
            {
                NotifyException(new ApplicationException("Failed during tag initialization", ex));
            }

            PersistanceService.Start(this);
            RawMessagePollingService.Start(this);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (LastException != null && !IsProductionDeployment && HaltOnBackgroundExceptionsInDev)
            {
                throw LastException;
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        public static IKernel Kernel
        {
            get { return Offr.Global.Kernel;  }
        }

        #region Implementation of IBackgroundExceptionReceiver

        public void NotifyException(Exception ex)
        {
            _log.Error(ex);
            LastException = ex;
        }

        public Exception LastException
        {
            get;
            private set;
        }

        #endregion
    }
}