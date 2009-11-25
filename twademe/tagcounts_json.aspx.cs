using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Json;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public partial class tagcounts_json : System.Web.UI.Page
    {
        private ITagRepository _tagProvider;

      
        protected void Page_Load(object sender, EventArgs e)
        {
            _tagProvider = Global.Kernel.Get<ITagRepository>();
            List<ITag> usedTags = _tagProvider.GetTagsFromNameValueCollection(Request.QueryString);
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();
            TagCounts tagCounts = queryExecutor.GetTagCountsForTags(usedTags);
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
