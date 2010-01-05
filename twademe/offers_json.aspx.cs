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
        private const int DEFAULT_COUNT = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            NameValueCollection request = Request.QueryString;

            int messageCount;
            if (!int.TryParse(Request["count"], out messageCount))
            {
                messageCount = DEFAULT_COUNT;
            }
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(request);
            SendJSON(GetOffersJson(tags, messageCount));
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

        public static string GetOffersJson(List<ITag> tags, int messageCount)
        {
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageRepository>();
            IEnumerable<IMessage> messages = queryExecutor.GetMessagesForTags(tags);
            return GetOffersJson(messages, messageCount);
        }

        private static string GetOffersJson(IEnumerable<IMessage> messages, int messageCount)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new MessageListSerializer() });
            List<IMessage> messagesToSend = new List<IMessage>(messages);
            messagesToSend = (messagesToSend.Count <= messageCount) ? messagesToSend : messagesToSend.GetRange(0, DEFAULT_COUNT - 1);
            return serializer.Serialize(messagesToSend);
        }
    }
}