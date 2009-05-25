using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.OAuth;

namespace twademe.oauth
{
    public partial class postback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OAuthTwitter oAuth = new OAuthTwitter();

            if (Request["oauth_token"] != null)
            {
                //Get the access token and secret.
                oAuth.AccessTokenGet(Request["oauth_token"]);
                if (oAuth.TokenSecret.Length > 0)
                {
                    //We now have the credentials, so make a call to the Twitter API.
                    //url = "http://twitter.com/account/verify_credentials.xml";
                    //xml = oAuth.oAuthWebRequest(OAuthTwitter.Method.GET, url, String.Empty);
                    //apiResponse.InnerHtml = Server.HtmlEncode(xml);
                    TwitterAuth.StoreSession(oAuth);
                    string redirect = Session["next_redirect"] as string;
                    redirect = redirect ?? "/";
                    Response.Redirect(redirect);
                    ////POST Test
                    //url = "http://twitter.com/statuses/update.xml";
                    //xml = oAuth.oAuthWebRequest(OAuthTwitter.Method.POST, url, "status=" + Server.UrlEncode("Hello @swhitley - Testing the .NET oAuth API"));
                    //apiResponse.InnerHtml = Server.HtmlEncode(xml);
                }
            }
            else
            {
                Response.Redirect("oauth_fail.aspx");
            }
        }
    }
}