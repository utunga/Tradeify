﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Json;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class offers_json : System.Web.UI.Page
    {
        private ITagRepository _tagProvider;

        private const int DEFAULT_COUNT = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            _tagProvider = Global.Kernel.Get<ITagRepository>();

            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(Request.QueryString); 
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();
            IEnumerable<IMessage> messages = queryExecutor.GetMessagesForTags(tags);

            Response.ContentType = "application/json";
            SendJSON(messages, DEFAULT_COUNT);
        }


        private void SendJSON(IEnumerable<IMessage> messages, int count)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Register the custom converter.
            serializer.RegisterConverters(new JavaScriptConverter[] {new MessageListSerializer()});
            List<IMessage> messagesToSend = new List<IMessage>(messages.Take(count));

            if (null != Request.Params["jsoncallback"])
            {
                Response.Write(Request.Params["jsoncallback"] + "(" + serializer.Serialize(messagesToSend) + ")");
            }
            else
            {
                Response.Write(serializer.Serialize(messagesToSend));
            }
        }
    }
}
