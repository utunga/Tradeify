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
        private ITagRepository _tagProvider;
        private IMessageQueryExecutor _queryExecutor;
        private const int DEFAULT_COUNT = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            _tagProvider = Global.Kernel.Get<ITagRepository>();
            _queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();

            TagCounts availableTags ;
            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(Request.QueryString);

            if (tags.Count == 0)
            {
                // at start the 'available tags' is just all the tags
                availableTags = _queryExecutor.GetTagCounts();
            }
            else
            {
                // after that available tags is the tag counts for the current messages
                // so first get the current 'messages' based on tags (NOTE2J ideally we'd be caching this result since we only just calc'd it in offers_json.aspx!)
                IEnumerable<IMessage> messages = _queryExecutor.GetMessagesForTags(tags);
                StaticTagDex oneOffTagDex = new StaticTagDex(messages); // counts up the tags for these messages
                availableTags = oneOffTagDex.GetTagCounts();
            }
            Response.ContentType = "application/json";
            //tagcounts stuff
            _tagProvider = Global.Kernel.Get<ITagRepository>();
            List<ITag> usedTags = _tagProvider.GetTagsFromNameValueCollection(Request.QueryString);
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();
            TagCounts tagCounts = queryExecutor.GetTagCountsForTags(usedTags);
            Response.ContentType = "application/json";
            SendJSON(availableTags,tagCounts);
        }

        private void SendJSON(TagCounts tags_json,TagCounts tagcounts_json)
        {
            JavaScriptSerializer Tags = new JavaScriptSerializer();
            // Register the custom converter.
            Tags.RegisterConverters(new JavaScriptConverter[] {new TagCountsSerializer()});
            //JavaScriptSerializer TagCounts = new JavaScriptSerializer();
            // Register the custom converter.
            //Tags.RegisterConverters(new JavaScriptConverter[] { new TagCountsSerializer() });
            //  List<IMessage> messagesToSend = new List<IMessage>(messages.Take(count));
            string output = "{\"tags_json\":" + Tags.Serialize(tags_json) + ",\"tagcounts_json\":" +
                            Tags.Serialize(tagcounts_json)+"}";
            Response.Write(output);
        }

    }
}
