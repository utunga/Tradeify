﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Json;
using Offr.Query;
using Offr.Text;

namespace twademe
{
    public partial class tags_json : System.Web.UI.Page
    {
        private ITagProvider _tagProvider;

        private const int DEFAULT_COUNT = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            _tagProvider = Global.Kernel.Get<ITagProvider>();

            MessageQuery query = MessageQuery.MessageQueryFromNameValCollection(_tagProvider, Request.QueryString);
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();
            TagCounts tagCounts = queryExecutor.GetTagCountsForQuery(query);
            Response.ContentType = "application/json";
            SendJSON(tagCounts);
        }

        private void SendJSON(TagCounts tagCounts)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Register the custom converter.
            serializer.RegisterConverters(new JavaScriptConverter[] {new TagCountsSerializer()});
            //  List<IMessage> messagesToSend = new List<IMessage>(messages.Take(count));
            Response.Write(serializer.Serialize(tagCounts));
        }
    }
}