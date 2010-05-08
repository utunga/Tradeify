using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NLog;
using Offr.Common;

namespace Offr.Tests
{
    /// <summary>
    /// Class to use when you want run tests fast, or offline, without actually having to 
    /// use the internet 
    /// </summary>
    public class MockWebRequestFactory : IWebRequestFactory
    {

        private Dictionary<string, string> _seenURLS;
        private readonly WebRequestFactory _webRequest; // for making *real* web requests, when needed

        public string FilePath { get; set; }

        public MockWebRequestFactory()
        {
            _webRequest = new WebRequestFactory();
            FilePath = "data/offline_web_requests";

            try
            {
                InitializeFromFile();
            }
            catch (IOException)
            {
                // start blank again
                _seenURLS = new Dictionary<string, string>();
            }
        }

        public string RetrieveContent(string url)
        {
            if (!_seenURLS.ContainsKey(url))
            {
                _seenURLS[url] = _webRequest.RetrieveContent(url);
                SerializeToFile();
            }
            return _seenURLS[url];
        }

        public void SerializeToFile()
        {
            if (FilePath == null) throw new ApplicationException("Please set the FilePath before calling this method");
            LogManager.GetLogger("Global").Info("serializing " + this.ToString());

            lock (this)
            {
                Stream stream = File.Open(FilePath, FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, _seenURLS);
                stream.Close();
            }
        }

        public void InitializeFromFile()
        {

            if (FilePath == null) throw new ApplicationException("Please set the FilePath before calling this method");
            FileInfo jsonFile;
            if (!(jsonFile = new FileInfo(FilePath)).Exists)
            {
                throw new IOException("Cannot find file " + FilePath);
            }

            lock (this)
            {

                Dictionary<string, string> objectToSerialize;
                Stream stream = File.Open(FilePath, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                objectToSerialize = (Dictionary<string, string>)bFormatter.Deserialize(stream);
                stream.Close();
                _seenURLS = objectToSerialize;
            }
        }
    }
}
