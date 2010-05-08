using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Ninject;
using NLog;
using Offr.Common;
using Offr.Json;
using Offr.Message;
using Offr.Repository;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterRawMessageProvider : IRawMessageProvider
    {
        private readonly IWebRequestFactory _webRequest;

        private readonly IRawMessageReceiver _receiver;
        public string ProviderNameSpace
        {
            get { return "twitter"; }
        }
     
        public TwitterRawMessageProvider(IRawMessageReceiver receiver, IWebRequestFactory requestFactory)
        {
            _receiver = receiver;
            _webRequest = requestFactory;
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

        [Obsolete("not part of IRawMessageProvider any more")]
        public IEnumerable<IRawMessage> ForQueryText(string keywordQuery)
        {
            // always require the "hash tag of the messages we are interested in "
            string query = GetQuery();
            if (keywordQuery != null)
            {
                query += "+" + HttpUtility.UrlEncode(keywordQuery);
            }
            string url = String.Format((string) TWITTER_SEARCH_INIT_URI, (object) query); // query back as far as we can go for these keywords 
            LogManager.GetLogger("Global").Info("Polling twitter now");
            //Console.WriteLine("url =" + url);
            string responseData = _webRequest.RetrieveContent(url);
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

        [Obsolete("not part of IRawMessageProvider any more")]
        public IRawMessage ByID(string providerMessageID)
        {
            string url = String.Format((string) TWITTER_STATUS_URI, (object) providerMessageID); // query back to beginning of time
            string responseData = _webRequest.RetrieveContent(url);
            TwitterStatusXml xmlStatus = XmlTwitterParser.ParseStatus(responseData);
            return new TwitterRawMessage(xmlStatus);
        }

        private long _last_id = 0;
        private IList<IRawMessage> GetNewUpdates()
        {

            // always require the "hash tag of the messages we are interested in "
            string query = GetQuery();
            string url = _last_id == 0 ? 
                String.Format((string) TWITTER_SEARCH_INIT_URI, (object) query) :
                String.Format(TWITTER_SEARCH_POLL_URI, _last_id, query);
            
            TwitterResultSet resultSet = null;
            try
            {
                string responseData = _webRequest.RetrieveContent(url);
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
            //"%23offer+OR+%23offr+OR+%23wanted+OR+%23want+OR+%23wants";
            ITagRepository tags = Global.GetTagRepository();
            List<ITag> groups=tags.GetGroups();
            string request="";
            if(groups.Count>=1)
            {
                request = groups[0].Text;
                for(int i=1;i<groups.Count;i++)
                {
                    request += "+OR+" + groups[i].Text;
                }
            }
            return request;
            //return QUERY_FOR_GROUPS;
        }

        public const string TWITTER_SEARCH_INIT_URI = "http://search.twitter.com/search.json?q={0}&rpp=100";
        public const string TWITTER_SEARCH_POLL_URI = "http://search.twitter.com/search.json?since_id={0}&q={1}";
        public const string TWITTER_USER_URI = "http://twitter.com/users/show/{0}.xml";
        public const string TWITTER_STATUS_URI = "http://twitter.com/statuses/show/{0}.xml";
    }
}
