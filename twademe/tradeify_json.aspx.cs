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
            IUserPointer user = GetUserPointer(request);
            MessagesWithTagCounts messagesWithTags = queryExecutor.GetMessagesWithTagCounts(tags,user);
            
            //string offers = offers_json.GetOffersJson(messagesWithTags);
            //string tagString = tags_json.GetTagJson(tags,messagesWithTags);
            //string tradeifyJson = "{\"offers_json\":" + offers + ",\"tags_json\":" +
            //                tagString + "}";
            SendJSON(JSON.Serialize(messagesWithTags));
        }

        private IUserPointer GetUserPointer(NameValueCollection nameVals)
        {
            if(Request.Form["username"]!=null && Request.Form["namespace"]!=null)
            {
                return new OpenSocialUserPointer()
                {
                    ProviderNameSpace = Request["namespace"],
                    ProviderUserName =  Request["username"]
                };
            }
            else return null;
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
