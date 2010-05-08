using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr;
using Offr.Common;
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
            NameValueCollection request = Request.QueryString;
            ITagRepository _tagProvider = Global.GetTagRepository();

            TagType? tagType = null;
            if (Request["type"] != null)
            {
                TagType parsed;
                if (Enums.TryParse<TagType>(Request["type"], out parsed))
                {
                    tagType = parsed;
                }
            }

            List<ITag> tags = _tagProvider.GetTagsFromNameValueCollection(request);
            IMessageQueryExecutor _queryExecutor = Global.GetMessageRepository();
            IEnumerable<TagWithCount> suggestedTags = _queryExecutor.GetSuggestedTags(tags, tagType);
            SendJSON(JSON.Serialize(suggestedTags));
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
