using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr;

namespace twademe
{
    public partial class RemoveMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] != null)
            {
                IMessageRepository messageRepository = Global.Kernel.Get<IMessageRepository>();
                if (messageRepository.Get(Request["id"]) != null)
                {
                    messageRepository.Remove(Request["id"]);
                    Response.Write("{\"status\":\"success\"}");
                }
                else SendJSON("{\"status\":\"id not found\"}");
            }
            else SendJSON("{\"status\":\"id is empty\"}");
        }
        private void SendJSON(string message)
        {
            if (null != Request.Params["jsoncallback"])
            {
                Response.Write(Request.Params["jsoncallback"] + "(" + message + ")");
            }
            else
            {
                Response.Write(message);
            }
        }
    }
}
