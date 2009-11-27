using System;
using System.Collections.Generic;
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
    public class Global : System.Web.HttpApplication
    {
        public const string INITIAL_OFFERS_FILE = "/data/initial_offers.json";
        public const string INITIAL_TAGS_FILE = "/data/initial_tags.json";
        public static Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start(object sender, EventArgs e)
        {
            IMessageRepository messageRepository = Kernel.Get<IMessageRepository>();
            if (messageRepository is IPersistedRepository)
            {
                ((IPersistedRepository)messageRepository).FilePath = Server.MapPath(INITIAL_OFFERS_FILE);
                ((IPersistedRepository)messageRepository).InitializeFromFile();
            }
            ITagRepository tagRepository = Kernel.Get<ITagRepository>();
            if (tagRepository is IPersistedRepository)
            {
                ((IPersistedRepository)tagRepository).FilePath = Server.MapPath(INITIAL_TAGS_FILE);
                ((IPersistedRepository)tagRepository).InitializeFromFile();
            }
            PersistanceService.EnsureStarted(new BackgroundExceptionReceiver());
            TwitterPollingService.EnsureStarted(new BackgroundExceptionReceiver());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
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
    }
}