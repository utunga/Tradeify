using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Ninject;
using NLog;
using Offr.Json;
using Offr.Message;
using Offr.Repository;
using Offr.Text;
using Offr.Common;

namespace Offr.RSS
{
    public class RSSRawMessageProvider : IRawMessageProvider
    {
        private readonly IRawMessageReceiver _receiver;
        private readonly IWebRequestFactory _webRequest;
        private readonly IRSSRawMessageRepository _seenRSSUpdates;
        private readonly string _rssFeedURI;
        private readonly string _providerNameSpace;
        
        public string ProviderNameSpace
        {
            get { return _providerNameSpace; }
        }
  
        public RSSRawMessageProvider(string providerNameSpace, string rssFeedURI, IRawMessageReceiver receiver, IRSSRawMessageRepository seenRSSUpdates, IWebRequestFactory webRequestFactory)
        {
            _webRequest = webRequestFactory;
            _providerNameSpace = providerNameSpace;
            _rssFeedURI = rssFeedURI;
            _receiver = receiver;
            _seenRSSUpdates = seenRSSUpdates;
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
                string responseData = _webRequest.RetrieveContent(_rssFeedURI);
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
                RSSRawMessage tmp = new RSSRawMessage(_rssFeedURI, rssItem);
                if (!_seenRSSUpdates.Has(tmp.ID))
                {
                    // this GUID hasn't been seen before
                    // so notify (note this doesn't mean its a valid message)
                    // might also be a semantic duplicate of something posted in another way
                    _seenRSSUpdates.Save(tmp);
                    newStatusUpdates.Add(tmp);
                }
            }
            _lastUpdatedTime = updatedTime; 
            return newStatusUpdates;
        }
    }
}
