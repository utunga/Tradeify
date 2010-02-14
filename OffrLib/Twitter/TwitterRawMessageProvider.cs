using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Ninject.Core;
using NLog;
using Offr.Json;
using Offr.Message;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterRawMessageProvider : IRawMessageProvider
    {
        private readonly IRawMessageReceiver _receiver;

        public TwitterRawMessageProvider(IRawMessageReceiver receiver)
        {
            _receiver = receiver;
        }
        /*
        [Inject]
        public TwitterRawMessageProvider(IRawMessageReceiver receiver) : this(receiver)
        {
           
        }
        */
        #region Implementation of IRawMessageProvider

        // not sure how useful this is
        public string ProviderNameSpace
        {
            get { return "twitter"; }
        }


        public void Update()
        {
            IList<IRawMessage> updates = GetNewUpdates();
            if (updates.Count > 0)
            {
                _receiver.Notify(updates);
            }
        }

        public IEnumerable<IRawMessage> ForQueryText(string keywordQuery)
        {
            // always require the "hash tag of the messages we are interested in "
            string query = GetQuery();
            if (keywordQuery != null)
            {
                query += "+" + HttpUtility.UrlEncode(keywordQuery);
            }
            string url = String.Format(WebRequest.TWITTER_SEARCH_INIT_URI, query); // query back as far as we can go for these keywords 
            LogManager.GetLogger("Global").Info("Polling twitter now");
            //Console.WriteLine("url =" + url);
            string responseData = WebRequest.RetrieveContent(url);
            Console.WriteLine("responseData =" + responseData);
            TwitterResultSet resultSet = JSON.Deserialize<TwitterResultSet>(responseData);
            //note that we don't update _last_id in this case (we're not caching this data)
            List<IRawMessage> statusUpdatesForQuery = new List<IRawMessage>();
            foreach (TwitterStatus status in resultSet.results)
            {
                statusUpdatesForQuery.Add(new TwitterRawMessage(status));
            }
            return statusUpdatesForQuery;
        }

        public IRawMessage ByID(string providerMessageID)
        {
            string url = String.Format(WebRequest.TWITTER_STATUS_URI, providerMessageID); // query back to beginning of time
            string responseData = WebRequest.RetrieveContent(url);
            TwitterStatusXml xmlStatus = XmlTwitterParser.ParseStatus(responseData);
            return new TwitterRawMessage(xmlStatus);
        }

        #endregion

        private long _last_id = 0;
        private IList<IRawMessage> GetNewUpdates()
        {

            // always require the "hash tag of the messages we are interested in "
            string query = GetQuery();
            string url = _last_id == 0 ? 
                String.Format(WebRequest.TWITTER_SEARCH_INIT_URI, query) :
                String.Format(WebRequest.TWITTER_SEARCH_POLL_URI, _last_id, query);
            
            TwitterResultSet resultSet = null;
            try
            {
                string responseData = WebRequest.RetrieveContent(url);
                resultSet = JSON.Deserialize<TwitterResultSet>(responseData);
                _last_id = resultSet.max_id;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                      //no internet, return blank results
                    return new List<IRawMessage>();
                }
                //handle rate limitig in case of excessive requests
            }

            if ((resultSet == null)  || resultSet.results == null)
            {
                //something else went wrong, return blank results
                return new List<IRawMessage>();
            }

            List<IRawMessage> newStatusUpdates = new List<IRawMessage>();   
            foreach (TwitterStatus status in resultSet.results)
            {
                newStatusUpdates.Add(new TwitterRawMessage(status));
            }
            return newStatusUpdates;
        }

        private string GetQuery()
        {
            /*
            switch (forType)
            {
                case MessageType.offer:
                    // filter to include any of these hash tags 
                    //return "%23ihave+OR+%23offer+OR+%23offr";
                    return "%23offer+OR+%23offr";
                default:
                    return "%23" + _forType;
            }
             */
            return "%23offer+OR+%23offr+OR+%23wanted+OR+%23want+OR+%23wants";
        }
    }
}
