using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Offr.Message;

namespace twademe
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["next_redirect"] = "/";
        }

        //[WebMethod]
        //public static IEnumerable GetOffers(int Count)
        //{
        //    IMessageProvider msgProvider = Global.Kernel.Get<IMessageProvider>();
        //    List<IMessage> messages = new List<IMessage>(msgProvider.AllMessages);

        //    // create an anonymous type with just the data we want for each message
            
        //    var messageData =
        //      from msg in messages
        //      select new
        //      {
        //          type = msg.MessageType.ToString(),
        //          offered_by = msg.CreatedBy.ProviderUserName,
        //          timestamp = msg.TimeStamp,
        //          end_by = ((OfferMessage) msg).EndByText,
        //          tags = ((OfferMessage)msg).Tags,
        //          offer_text = ((OfferMessage)msg).MessageText
        //      };

        //    return messageData.Take(Count);
        //}
    }
}
