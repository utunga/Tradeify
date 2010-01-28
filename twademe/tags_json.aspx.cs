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
    public partial class tags_json : System.Web.UI.Page
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            NameValueCollection request = Request.QueryString;
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(request);
            SendJSON(GetTagJson(tags));
        }

        public static string GetTagJson(List<ITag> tags)
        {
            IMessageQueryExecutor _queryExecutor = Global.Kernel.Get<IMessageRepository>();
            MessagesWithTagCounts messagesWithTags = _queryExecutor.GetMessagesWithTagCounts(tags);
            return SerializeTagsOnly(messagesWithTags);
        }

        private static string SerializeTagsOnly(MessagesWithTagCounts messagesWithTags)
        {
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.RegisterConverters(new JavaScriptConverter[] { new TagCountsSerializer() });

            return "{\"tags_json\":" + JSON.Serialize(messagesWithTags.Tags) + ",\"tagcount\":" +
                   JSON.Serialize(messagesWithTags.TagCount) + "}";
        }
    }
}
