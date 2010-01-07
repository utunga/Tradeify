using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Json;
using Offr.Message;
using Offr.OAuth;
using Offr.Text;
using Offr.OpenSocial;

namespace twademe
{
    public partial class parse : System.Web.UI.Page
    {
        private IMessageParser _messageParser;
        public parse()
        {
            _messageParser = Global.Kernel.Get<IMessageParser>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string messageText = Request.Form["message"] ?? Request.QueryString["message"];
            if (messageText != null)
            {
                var messageWrapper = new TextWrapperRawMessage(messageText);
                IMessage message = _messageParser.Parse(messageWrapper);
                SendJSON(message);
            }
        }

        private void SendJSON(IMessage message)
        {
            Response.ContentType = "application/json";

            /*Dictionary<string, object> parsedOutput = new Dictionary<string, object>();
            parsedOutput.Add("message", message);
            List<string> failReasons = new List<string>();
            parsedOutput.Add("validationFailReasons", message.ValidationFailReasons());*/
            string messageJson = "{\"message\":" + JSON.Serialize(message) + ",\"validationFailReasons\":" +
                  JSON.Serialize(message.ValidationFailReasons()) + "}";//JSON.Serialize(parsedOutput);
           messageJson = Regex.Replace(messageJson, @"\s", "");
            if (null != Request.Params["jsoncallback"])
            {
                Response.Write(Request.Params["jsoncallback"] + "(" + messageJson + ")");
            }
            else
            {
                Response.Write(messageJson);
            }
            Response.End();
            Response.Flush();
        }
    }
}