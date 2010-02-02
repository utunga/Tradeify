using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class tradeify_json : System.Web.UI.Page
    {

    
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            NameValueCollection request = Request.QueryString;
            
            ITagRepository tagProvider = Global.Kernel.Get<ITagRepository>();
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageRepository>();
            List<ITag> tags = tagProvider.GetTagsFromNameValueCollection(request);
            MessagesWithTagCounts messagesWithTags = queryExecutor.GetMessagesWithTagCounts(tags);
            
            //string offers = offers_json.GetOffersJson(messagesWithTags);
            //string tagString = tags_json.GetTagJson(tags,messagesWithTags);
            //string tradeifyJson = "{\"offers_json\":" + offers + ",\"tags_json\":" +
            //                tagString + "}";
            SendJSON(JSON.Serialize(messagesWithTags));
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
