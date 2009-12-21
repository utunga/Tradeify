﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Offr.Twitter
{
    public static class WebRequest
    {
        public const string TWITTER_SEARCH_INIT_URI = "http://search.twitter.com/search.json?q={0}&rpp=100";
        public const string TWITTER_SEARCH_POLL_URI = "http://search.twitter.com/search.json?since_id={0}&q={1}";
        public const string TWITTER_USER_URI = "http://twitter.com/users/show/{0}.xml";
        public const string TWITTER_STATUS_URI = "http://twitter.com/statuses/show/{0}.xml";
        
        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <returns>The web server response.</returns>
        public static string RetrieveContent(string url)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "TwadeMe";
            webRequest.Timeout = 20000;
            
            string responseData = "";
            
            StreamReader responseReader = null;
            WebResponse response = null;
            try
            {
                response = webRequest.GetResponse();
                responseReader = new StreamReader(response.GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw; //FIXME
            }
            finally
            {
                if (response != null)
                {
                    response.GetResponseStream().Close();
                }
                if (responseReader != null)
                {
                    responseReader.Close();
                }
            }

            return responseData;
        }
    }
}
