using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.OAuth;
using Offr.Text;

namespace twademe
{
    public partial class accept_post : System.Web.UI.Page
    {
        private static readonly OpenSocialMessageProvider Provider = new OpenSocialMessageProvider();
        private static List<MessageWrapper> _posts = new List<MessageWrapper>();

        public string DebugData
        {
         
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (MessageWrapper post in _posts)
                {
                    sb.Append("<table><tr><td colspan=2><b>POST</b></td></tr>");
                    foreach (string key in post.Data.AllKeys)
                    {
                        sb.Append("<tr><td><b>" + key + "</b></td><td>" + post.Data[key]+ "</td></tr>");
                    }
                    sb.Append("</table>");
                }
                return sb.ToString();
            }
        }

        //private static List<String> rawMessage = new List<String>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                MessageWrapper wrapper = new MessageWrapper();
                foreach (string key in Request.Form.AllKeys)
                {
                    wrapper.Data.Add("REQ:" + key, Request.Form[key]);
                }
                //foreach (string key in Request.Headers.AllKeys)
                //{
                //    wrapper.Data.Add("HDR:" + key, Request.Headers[key]);
                //}
                foreach (string key in Request.QueryString.AllKeys)
                {
                    wrapper.Data.Add("QRY:" + key, Request.QueryString[key]);
                }
                string message = Request.Form["RawMessage"];
                string userName = Request.Form["UserName"] ?? "unknown";
                _posts.Add(wrapper);
            }


        }

        private class MessageWrapper
        {
            public NameValueCollection Data = new NameValueCollection();
            public string Raw { get; set; }
            public string Message { get; set; }
        }
    }
}
