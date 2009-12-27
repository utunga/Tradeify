using System;
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
    public partial class tags_json : System.Web.UI.Page
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            NameValueCollection collection = Request.QueryString;
            Response.Write(GetTagJson(collection));
        }

        public static string GetTagJson(NameValueCollection request)
        {
            ITagRepository _tagProvider = Global.Kernel.Get<ITagRepository>();
            IMessageQueryExecutor _queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();

            TagCounts availableTags ;
            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(request);

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
