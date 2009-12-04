using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.OAuth;

namespace twademe
{
    public partial class accept_post : System.Web.UI.Page
    {
        private static List<MessageWrapper> posts = new List<MessageWrapper>();
        //private static List<String> rawMessage = new List<String>();
        protected void Page_Load(object sender, EventArgs e)
        {
            MessageWrapper wrapper= new MessageWrapper();
            NameValueCollection collection = Request.QueryString;
            NameValueCollection form = Request.Form;
            wrapper.raw = "";//= collection.ToString();
            foreach (string key in form)
                wrapper.raw += key + ",";
            if (form["RawMessage"]!=null)
            {
                wrapper.message = form["RawMessage"].ToString();
                
            }
            posts.Add(wrapper);
            foreach (MessageWrapper w in posts)
            {
                Response.Write(" | " + w.raw + " : " + w.message + " | ");
            }
        }
        private class MessageWrapper
    {
            public string raw{ get; set;}
            public string message { get; set; }
    }
    }
}
