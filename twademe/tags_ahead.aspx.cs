using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Common;
using Offr.Json;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public const int MAX_RESULTS = 20;

        protected void Page_Load(object sender, EventArgs e)
        {
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();

            if (Request.Params["type"] != null || Request.Params["q"] !=null)
            {
                TagType type = Request.Params["type"] == null ? TagType.tag : Request.Params["type"].ToEnum(TagType.tag);

                string prefix = Request.Params["q"] ?? "";

                List<string> tags = _tagProvider.GetTagsFromTypeAhead(prefix, type, MAX_RESULTS);

           
                string tagString = "";
                foreach (var tag in tags)
                {
                    tagString += tag + "\n";
                }
                SendJSON(tagString);
                Response.Flush();
                Response.End();
           
            }
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
