using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace twademe
{
    public partial class tradeify_json : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            NameValueCollection request = Request.QueryString;
            string offers = offers_json.GetOffersJson(request);
            string tags = tags_json.GetTagJson(request);
            string tradeifyJson = "{\"offers_json\":" + offers + ",\"tags_json\":" +
                            tags + "}";
            Response.Write(tradeifyJson);
            
        }
    }
}
