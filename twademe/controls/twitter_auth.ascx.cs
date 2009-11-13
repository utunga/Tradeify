using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.OAuth;
using Offr.Users;

namespace twademe.controls
{
    public partial class twitter_auth : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TwitterAuth.HasValidSession())
            {
                InitalizeAuth.Visible = true;
                SignedIn.Visible = false;
            }
            else
            {
                InitalizeAuth.Visible = false;
                SignedIn.Visible = true;
                User twitterUser = TwitterAuth.CurrentUser;
                ProfilePic.Src = twitterUser.ProfileImageUrl;
                ProfileScreenName.InnerText = twitterUser.ScreenName;

            }

        }

        protected void SignInWithTwitter_Click(object sender, ImageClickEventArgs e)
        {
            OAuthTwitter oAuth = new OAuthTwitter();
            //Redirect the user to Twitter for authorization.
            //Using oauth_callback for local testing.
            Response.Redirect(oAuth.AuthorizationLinkGet());
        }
    }
}