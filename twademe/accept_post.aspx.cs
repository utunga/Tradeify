using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NLog;
using Offr.OAuth;
using Offr.Text;
using Offr.OpenSocial;

namespace twademe
{
    public partial class accept_post : System.Web.UI.Page
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private static List<Post> _posts = new List<Post>();
        public string DebugData
        {
           
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Post post in _posts)
                {
                    sb.Append("<table><tr><td colspan=2><b>POST</b></td></tr>");
                    foreach (string key in post.Data.AllKeys)
                    {
                        sb.Append("<tr><td><b>" + key + "</b></td><td>" + post.Data[key]+ "</td></tr>");
                    }
                    sb.Append("<tr><td><b>username</b></td><td>" + post.Username + "</td></tr>");
                    sb.Append("<tr><td><b>post</b></td><td>" + post.ToString() + "</td></tr>");
                    sb.Append("</table>");
                }
                return sb.ToString();
            }
        }

        //private static List<String> rawMessage = new List<String>();
        protected void Page_Load(object sender, EventArgs e)
        {

            Post wrapper = new Post();
            foreach (string key in Request.Form.AllKeys)
            {
                wrapper.Data.Add("REQ:" + key, Request.Form[key]);
            }
            foreach (string key in Request.Headers.AllKeys)
            {
                wrapper.Data.Add("HDR:" + key, Request.Headers[key]);
            }
            foreach (string key in Request.QueryString.AllKeys)
            {
                wrapper.Data.Add("QRY:" + key, Request.QueryString[key]);
            }



            string message = Request.Form["message"] ?? Request["message"];
            string username = Request.Form["username"] ?? Request["username"] ?? "unknown";
            string nameSpace = Request.Form["namespace"] ?? Request["namespace"] ?? "ooooby";
            string thumbnail = Request.Form["thumbnail"] ?? Request["thumbnail"] ?? "unknown";
            string profileUrl = Request.Form["profileurl"] ?? Request["profileurl"] ?? "unknown"; 

            wrapper.Username = username;
            wrapper.Msg = message;
            wrapper.NameSpace = nameSpace;
            wrapper.ProfileUrl = profileUrl;
            wrapper.Thumbnail = thumbnail;
            _posts.Add(wrapper);

            if (!string.IsNullOrEmpty(message))
            {
                _log.Info("Request received:" + wrapper);
                Global.NotifyRawMessage(new OpenSocialRawMessage(nameSpace, message, username, thumbnail, profileUrl));
            }

        }

        private class Post
        {
            public NameValueCollection Data = new NameValueCollection();
            public string Msg { get; set; }
            public string Thumbnail { get; set; }
            public string Username { get; set; }
            public string NameSpace { get; set; }
            public string ProfileUrl { get; set; }
            public override string ToString()
            {
                return string.Format("msg:'{0}' user:'{1}' namespace:{2} profileUrl:{3} ", Msg, Username, NameSpace, ProfileUrl);
            }
        }
    }
}
