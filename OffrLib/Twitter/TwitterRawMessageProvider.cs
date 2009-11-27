using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Script.Serialization;
using NLog;
using Offr.Message;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterRawMessageProvider : IRawMessageProvider
    {
        private MessageType _forType;
        private readonly List<IRawMessageReceiver> _receivers;

        public TwitterRawMessageProvider(MessageType forType)
        {
            _forType = forType;
            _receivers = new List<IRawMessageReceiver>();
        }
        
        //FIXME remove this
        public TwitterRawMessageProvider() : this(MessageType.offr)
        {
        }

        #region Implementation of IRawMessageProvider

        // not sure how useful this is
        public string ProviderNameSpace
        {
            get { return "twitter"; }
        }

        public void RegisterForUpdates(IRawMessageReceiver receiver)
        {
            if (!_receivers.Contains(receiver))
            {
                _receivers.Add(receiver);
            }
        }

        public void Update()
        {
            IList<IRawMessage> updates = GetNewUpdates();
            if (updates.Count > 0)
            {
                foreach (IRawMessageReceiver receiver in _receivers)
                {
                    receiver.Notify(updates);
                }
            }
        }

        public IEnumerable<IRawMessage> ForQueryText(string keywordQuery)
        {
            // always require the "hash tag of the messages we are interested in "
            string query = GetQuery(_forType);
            if (keywordQuery != null)
            {
                query += "+" + HttpUtility.UrlEncode(keywordQuery);
            }
            string url = String.Format(WebRequest.TWITTER_SEARCH_INIT_URI, query); // query back as far as we can go for these keywords 
            LogManager.GetLogger("Global").Info("Polling twitter now");
            //Console.WriteLine("url =" + url);
            string responseData = WebRequest.RetrieveContent(url);
            Console.WriteLine("responseData =" + responseData);
            TwitterResultSet resultSet = (new JavaScriptSerializer()).Deserialize<TwitterResultSet>(responseData);
            //note that we don't update _last_id in this case (we're not caching this data)
            List<IRawMessage> statusUpdatesForQuery = new List<IRawMessage>();
            foreach (TwitterStatus status in resultSet.results)
            {
                statusUpdatesForQuery.Add(RawMessage.From(status));
            }
            return statusUpdatesForQuery;
        }

        public IRawMessage ByID(string providerMessageID)
        {
            string url = String.Format(WebRequest.TWITTER_STATUS_URI, providerMessageID); // query back to beginning of time
            string responseData = WebRequest.RetrieveContent(url);
            Status xmlStatus = XmlTwitterParser.ParseStatus(responseData);
            return RawMessage.From(xmlStatus);
        }

        #endregion

        private long _last_id = 0;
        private IList<IRawMessage> GetNewUpdates()
        {

            // always require the "hash tag of the messages we are interested in "
            string query = GetQuery(_forType);
            string url = _last_id == 0 ? 
                String.Format(WebRequest.TWITTER_SEARCH_INIT_URI, query) :
                String.Format(WebRequest.TWITTER_SEARCH_POLL_URI, _last_id, query);

            string responseData = WebRequest.RetrieveContent(url);
            TwitterResultSet resultSet = (new JavaScriptSerializer()).Deserialize<TwitterResultSet>(responseData);
            _last_id = resultSet.max_id;

            List<IRawMessage> newStatusUpdates = new List<IRawMessage>();
            foreach (TwitterStatus status in resultSet.results)
            {
                newStatusUpdates.Add(RawMessage.From(status));
            }
            return newStatusUpdates;
        }

        private string GetQuery(MessageType forType)
        {
            switch (forType)
            {
                case MessageType.offr:
                    // filter to include any of these hash tags 
                    return "%23ihave+OR+%23offer+OR+%23offr";
                default:
                    return "%23" + _forType;
            }
        }
    }
}
