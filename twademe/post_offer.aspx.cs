using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.OAuth;
using Offr.Text;

namespace twademe
{
    public partial class post_offer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (TwitterAuth.HasValidSession())
            {
                AuthRequiredWarning.Visible = false;
                PostMessage.Visible = true;

                if (IsPostBack)
                {
                    string statusMessage = Status.Value;
                    if ((statusMessage != null) &&
                        !string.IsNullOrEmpty(statusMessage.Trim()))
                    {
                        const string url = "http://twitter.com/statuses/update.xml";
                        string xml = TwitterAuth.CurrentSession.
                            oAuthWebRequest(OAuthTwitter.Method.POST, url, "status=" + Server.UrlEncode(statusMessage));
                        Status.Disabled = true;
                        PostedMessageStatus.Visible = true;
                    }
                }
            }
            else
            {
                Session["next_redirect"] = "/post_offer.aspx";
                AuthRequiredWarning.Visible = true;
                PostMessage.Visible = true;
            }
            string messageText = Request.Form["Message"];
            IRawMessageReceiver messageReceiver = Global.Kernel.Get<IRawMessageReceiver>();
            if (messageText != null)
                messageReceiver.Notify(RawMessage.From(messageText, "100", "Unkown", ""));
        }
    }
}
