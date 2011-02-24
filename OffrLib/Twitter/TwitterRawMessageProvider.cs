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

        private long _last_id = 0;
        private IList<IRawMessage> GetNewUpdates()
        {

            // always require the "hash tag of the messages we are interested in "
            //string query = GetQuery();

            // bit to general for testing
            //"(#chch OR #eqnz) (offer OR need OR want OR wanted OR needed)"
            //string query = "(%23chch+OR+%23eqnz)+(offer+OR+need+OR+want+OR+wanted+OR+needed)";
            
            //(#chch OR #eqnz) (#offer OR #need)"
            string query = "(%23chch+OR+%23eqnz)+(%23offer+OR+%23need+OR+%23helpme)";
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

        //private string GetQuery()
        //{

        //    return "%23chch+OR+%23eqnz+OR+%23christchurch+OR+chchneeds";
        //    //ITagRepository tags = Global.GetTagRepository();
        //    //List<ITag> groups=tags.GetGroups();

        //    //string request="";
        //    //if(groups.Count>=1)
        //    //{
        //    //    request = groups[0].Text;
        //    //    for(int i=1;i<groups.Count;i++)
        //    //    {
        //    //        request += "+OR+" + groups[i].Text;
        //    //    }
        //    //}
        //    //return request;
        //    //return QUERY_FOR_GROUPS;
        //}

        public const string TWITTER_SEARCH_INIT_URI = "http://search.twitter.com/search.json?q={0}&rpp=100";
        public const string TWITTER_SEARCH_POLL_URI = "http://search.twitter.com/search.json?since_id={0}&q={1}";
        public const string TWITTER_USER_URI = "http://twitter.com/users/show/{0}.xml";
        public const string TWITTER_STATUS_URI = "http://twitter.com/statuses/show/{0}.xml";
    }
}
