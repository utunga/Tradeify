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
        public const int TYPE_AHEAD_NUMBER = 20;
        protected void Page_Load(object sender, EventArgs e)
        {
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            NameValueCollection request = Request.QueryString;
            List<string>tags=new List<string>();
            TagType? type = null;
            if (null != Request.Params["type"])
            {
                try
                {
                    type = (TagType) Enum.Parse(typeof (TagType), Request.Params["type"]);
                }catch(Exception ex){}
            }
            if (null != Request.Params["q"])
            {
                string q = Request.Params["q"];
                tags.AddRange(_tagProvider.GetTagsFromTypeAhead(q, type, TYPE_AHEAD_NUMBER));
            }
            /*
            foreach (var key in request)
            {
                string q=request.GetValues(key.ToString())[0];
                
                
            }*/
            string tagString = "";
            foreach (var tag in tags)
            {
                tagString += tag + "\n";
            }
            SendJSON(tagString);
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
