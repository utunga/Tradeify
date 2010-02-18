﻿using System;
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
                messageRepository.Remove(Request["id"]);
                Response.Write("success");
            }
            else Response.Write("failure");
        }
    }
}
