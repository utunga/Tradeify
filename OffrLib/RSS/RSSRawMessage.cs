using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using Newtonsoft.Json;
using Offr.Message;
using Offr.Text;

namespace Offr.RSS
{
    public class RSSRawMessage : RawMessage, IRawMessage
    {
        public RSSRawMessage(string sourceURI, SyndicationItem rssItem)
        {
            base.Pointer = new RSSMessagePointer(sourceURI, rssItem.Id);
            //base.CreatedBy = new TwitterUserPointer(status.from_user, status.profile_image_url);
            base.Text = rssItem.Summary.Text;
            //rssItem.Id;
            //base.Timestamp = rssItem.LastUpdatedTime; 
        }
    }

    public class RSSMessagePointer : MessagePointerBase
    {
        public override string ProviderNameSpace { 
            get; 
            protected set;
        }

        internal RSSMessagePointer()
        {
        }

        public RSSMessagePointer(string sourceURI, string id)
        {
            ProviderNameSpace = sourceURI;
            ProviderMessageID = id;
        }
    }
}
