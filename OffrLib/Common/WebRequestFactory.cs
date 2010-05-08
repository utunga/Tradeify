using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Offr.Common
{

    public class WebRequestFactory : IWebRequestFactory
    {

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <returns>The web server response.</returns>
        public string RetrieveContent(string url)
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