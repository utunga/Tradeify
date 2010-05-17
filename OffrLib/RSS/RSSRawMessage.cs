using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Offr.Repository;
using Offr.Text;

namespace Offr.RSS
{
    public class RSSRawMessage : RawMessage, IRawMessage, ITopic
    {
        public HtmlDocument Description { get; set; }
        public string ID { get; set; }
        public string Link0 { get; set; }
        public DateTime PublicationDate { get; set; }

        public RSSRawMessage(string sourceURI, SyndicationItem rssItem)
        {
            base.Pointer = new RSSMessagePointer(sourceURI, rssItem.Id);
            ID = rssItem.Id;
            var tmp = new HtmlDocument();
            tmp.LoadHtml(rssItem.Summary.Text);
            Description = tmp;
            Link0 = (rssItem.Links.Count > 0) ? rssItem.Links[0].GetAbsoluteUri().ToString() : null;
            PublicationDate = rssItem.PublishDate.UtcDateTime;
        }    }
}
