using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Ninject.Core;

namespace twademe
{
    public class Global : System.Web.HttpApplication
    {
        public const string INITIAL_OFFERS_FILE = "/data/initial_offers.json";

        protected void Application_Start(object sender, EventArgs e)
        {
            Offr.Global.Initialize(new NinjectKernelConfig());
            try
            {
                Offr.Global.InitializeWithRecentOffers(Server.MapPath(INITIAL_OFFERS_FILE));
            }
            catch (NotSupportedException ex)
            {
                if (ex.Message.Contains("one way converter currently"))
                {
                    //FIXME1 not supported for now but no point breaking the app so do nothing
                }
                else
                {
                    throw;
                }
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