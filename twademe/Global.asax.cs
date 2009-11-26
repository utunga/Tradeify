using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Ninject.Core;
using Offr;
using Offr.Message;
using Offr.Repository;

namespace twademe
{
    public class Global : System.Web.HttpApplication
    {
        public const string INITIAL_OFFERS_FILE = "/data/initial_offers.json";

        protected void Application_Start(object sender, EventArgs e)
        {
            IMessageRepository messageRepository = Kernel.Get<IMessageRepository>();
            if (messageRepository is IPersistedRepository)
            {
                ((IPersistedRepository)messageRepository).FilePath = Server.MapPath(INITIAL_OFFERS_FILE);
                ((IPersistedRepository)messageRepository).InitializeFromFile();
            }
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