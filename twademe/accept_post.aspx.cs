using System;
using System.Collections.Generic;
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
        private static List<String> posts = new List<String>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form["RawMessage"]!=null)
            {
                string message = Request.Form["RawMessage"].ToString();
                posts.Add(message);
                foreach (string m in posts)
                {
                    Response.Write(m+" | ");     
                }
                
            }
        }

    }
}
