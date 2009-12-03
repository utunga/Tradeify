using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.OAuth;

namespace twademe
{
    public partial class accept_post : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form["RawMessage"]!=null)
            {
                Response.Write(Request.Form["RawMessage"].ToString());
            }
        }
    }
}
