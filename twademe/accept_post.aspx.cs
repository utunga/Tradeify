﻿using System;
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
                    sb.Append("<tr><td><b>username</b></td><td>" + post.Username + "</td></tr>");
                    sb.Append("<tr><td><b>message</b></td><td>" + post.Raw + "</td></tr>");
                    sb.Append("<tr><td><b>thumbnail</b></td><td>" + post.Thumbnail + "</td></tr>");
                    sb.Append("</table>");
                }
                return sb.ToString();
            }
        }

        //private static List<String> rawMessage = new List<String>();
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Page.IsPostBack)
            //{

                MessageWrapper wrapper = new MessageWrapper();
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
                string messageText = Request.Form["RawMessage"];
                string userName = Request.Form["UserName"] ?? "unknown";
                string thumbnail = Request.Form["Thumbnail"] ?? "unknown";
                
                wrapper.Username = userName;
                wrapper.Raw = messageText;
                wrapper.Thumbnail = thumbnail;
                _posts.Add(wrapper);
            //}

                IRawMessageReceiver messageReceiver = Global.Kernel.Get<IRawMessageReceiver>();
                messageReceiver.Notify(RawMessage.From(messageText, "100", userName, thumbnail));

                //Provider.ParseMessage(message, userName, thumbnail);

        }

        private class MessageWrapper
        {
            public NameValueCollection Data = new NameValueCollection();
            public string Raw { get; set; }
            public string Thumbnail { get; set; }
            public string Username { get; set; }
        }
    }
}
