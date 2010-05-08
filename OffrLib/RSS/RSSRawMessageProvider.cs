using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Ninject.Core;
using NLog;
using Offr.Json;
using Offr.Message;
using Offr.Repository;
using Offr.Text;
using Offr.Common;
using WebRequest=Offr.Common.WebRequest;

namespace Offr.RSS
{
    public class RSSRawMessageProvider : IRawMessageProvider
    {
        private readonly IRawMessageReceiver _receiver;
        private readonly string _rssFeedURI;
        private readonly string _providerNameSpace;

        public string ProviderNameSpace
        {
            get { return _providerNameSpace; }
        }
        
        [Inject]
        public WebRequest.WebRequestMethod RetrieveWebContent { get; set; }

        public RSSRawMessageProvider(string providerNameSpace, string rssFeedURI, IRawMessageReceiver receiver)
        {
            _providerNameSpace = providerNameSpace;
            _rssFeedURI = rssFeedURI;
            _receiver = receiver;
        }
    
        #region Implementation of IRawMessageProvider

        public void Update()
        {
            IList<IRawMessage> updates = GetNewUpdates();
            if (updates.Count > 0)
            {
                _receiver.Notify(updates);
            }
        }

        #endregion

        private long _lastUpdatedTime = 0;
        private IList<IRawMessage> GetNewUpdates()
        {
            SyndicationFeed feed;
            long updatedTime = 0;
            try
            {
                string responseData = RetrieveWebContent(_rssFeedURI);
                XmlReader xmlReader = XmlReader.Create(new StringReader(responseData));
                feed = SyndicationFeed.Load(xmlReader);
                updatedTime = feed.LastUpdatedTime.Ticks;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                      //no internet, return blank results
                    return new List<IRawMessage>();
                }
                throw;
            }

            if (updatedTime <= _lastUpdatedTime)
            {
                //no new results..
                return new List<IRawMessage>();
            }

            List<IRawMessage> newStatusUpdates = new List<IRawMessage>();   
            foreach (SyndicationItem rssItem in feed.Items)
            {
                newStatusUpdates.Add(new RSSRawMessage(_rssFeedURI, rssItem));
            }
            _lastUpdatedTime = updatedTime; 
            return newStatusUpdates;
        }
    }
}
