using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Offr;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class offers_json : System.Web.UI.Page
    {

        private const int DEFAULT_COUNT = 10;

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.ContentType = "application/json";
            NameValueCollection request = Request.QueryString;
            SendJSON(GetOffersJson(request));
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
        
        public static string GetOffersJson(NameValueCollection request)
        {
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(request);
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageRepository>();
            IEnumerable<IMessage> messages = queryExecutor.GetMessagesForTags(tags);
            return GetOffersJson(messages);
        }

        private static string GetOffersJson(IEnumerable<IMessage> messages)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<IMessage> messagesToSend = new List<IMessage>(messages);
            messagesToSend = (messagesToSend.Count <= DEFAULT_COUNT) ? messagesToSend : messagesToSend.GetRange(0, DEFAULT_COUNT - 1);
            return serializer.Serialize(messagesToSend);
        }
    }
}