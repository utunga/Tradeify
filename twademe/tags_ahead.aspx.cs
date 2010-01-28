using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Json;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            NameValueCollection request = Request.QueryString;
            List<ITag>tags=new List<ITag>();
            foreach (var key in request)
            {
                string q=request.GetValues(key.ToString())[0];
                tags.AddRange(_tagProvider.GetTagsFromTypeAhead(q,null));
                
            }
            SendJSON(JSON.Serialize(tags));
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
