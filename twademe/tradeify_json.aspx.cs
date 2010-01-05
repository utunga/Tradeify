using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class tradeify_json : System.Web.UI.Page
    {
        private const int DEFAULT_MSG_COUNT = 10;

    
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            NameValueCollection request = Request.QueryString;
            
            int messageCount;
            if (!int.TryParse(Request["count"], out messageCount))
            {
                messageCount = DEFAULT_MSG_COUNT;
            }
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(request);
            string offers = offers_json.GetOffersJson(tags, messageCount);
            string tagString = tags_json.GetTagJson(tags);
            string tradeifyJson = "{\"offers_json\":" + offers + ",\"tags_json\":" +
                            tagString + "}";
            SendJSON(tradeifyJson);
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
