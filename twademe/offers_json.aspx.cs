using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Message;
using Offr.Query;

namespace twademe
{
    public partial class offers_json : System.Web.UI.Page
    {

        private const int DEFAULT_COUNT = 10;

        protected void Page_Load(object sender, EventArgs e)
        {

            MessageQuery query = MessageQuery.FromNameValCollection(Request.QueryString);
            Response.ContentType = "application/json"; 
            
            IMessageQueryExecutor queryExecutor = Global.Kernel.Get<IMessageQueryExecutor>();
            IEnumerable<IMessage> messages = queryExecutor.GetMessagesForQuery(query);
            SendJSON(messages, DEFAULT_COUNT);
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
