﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
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

            TagCounts availableTags ;

            if (tags.Count == 0)
            {
                // at start the 'available tags' is just all the tags
                availableTags = _queryExecutor.GetTagCounts();
            }
            else
            {
                // after that available tags is the tag counts for the current messages
                // so first get the current 'messages' based on tags (NOTE2J ideally we'd be caching this result since we only just calc'd it in tradeify_json.aspx!)
                IEnumerable<IMessage> messages = _queryExecutor.GetMessagesForTags(tags);
                TagDex oneOffTagDex = new TagDex(messages); // counts up the tags for these messages
                availableTags = oneOffTagDex.GetTagCounts();
            }
            
            //tagcounts stuff
            TagCounts tagCounts = _queryExecutor.GetTagCountsForTags(tags);
            return GetJsonOutput(availableTags, tagCounts);
        }

        private static string GetJsonOutput(TagCounts tags_json, TagCounts tagcounts_json)
        {
            JavaScriptSerializer Tags = new JavaScriptSerializer();
            Tags.RegisterConverters(new JavaScriptConverter[] {new TagCountsSerializer()});
            return "{\"tags_json\":" + Tags.Serialize(tags_json) + ",\"tagcounts_json\":" +
                   Tags.Serialize(tagcounts_json)+"}";
        }
    }
}
