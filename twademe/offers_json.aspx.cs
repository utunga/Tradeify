using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace twademe
{
    public partial class offers_json : System.Web.UI.Page
    {
        private ITagProvider _tagProvider;

        private const int DEFAULT_COUNT = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            _tagProvider = Global.Kernel.Get<ITagProvider>();

            MessageQuery query = MessageQueryFromNameValCollection(Request.QueryString);
            Response.ContentType = "application/json"; 
            
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();
            IEnumerable<IMessage> messages = queryExecutor.GetMessagesForQuery(query);
            SendJSON(messages, DEFAULT_COUNT);
        }

        private MessageQuery MessageQueryFromNameValCollection(NameValueCollection nameVals)
        {
            MessageQuery query = new MessageQuery();
            if (nameVals["q"]!=null)
            {
                query.Keywords = nameVals["q"];
            }
            foreach (TagType tagType in Enum.GetValues(typeof(TagType)))
            {
                if (nameVals.GetValues(tagType.ToString()) != null)
                {
                    foreach (string tagText in nameVals.GetValues(tagType.ToString()))
                    {
                        ITag tag = _tagProvider.FromTypeAndText(tagType, tagText);
                        query.Facets.Add(tag);
                    }
                }
            }
            return query;
        }

        private void SendJSON(IEnumerable<IMessage> messages, int count)
        {

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            // Register the custom converter.
            serializer.RegisterConverters(new JavaScriptConverter[] {new MessageListSerializer()});
            List<IMessage> messagesToSend = new List<IMessage>(messages.Take(count));
            Response.Write(serializer.Serialize(messagesToSend));
        }
    }
}
