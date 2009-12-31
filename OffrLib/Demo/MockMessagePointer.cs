using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Message;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Demo
{
    public class MockMessagePointer : IMessagePointer
    {
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }
        public string SourceURL { get; private set; }
        public IResolvedURI ResolvedURL { get; private set; }
        public string ProviderMessageID { get; private set; }
        public string ProviderNameSpace { get { return "twitter"; }}
        
        public MockMessagePointer(int id)
        {
            ProviderMessageID = id.ToString();
        }

        public MockMessagePointer(string url)
        {
            throw new NotImplementedException("Ideally you can add a message pointer by url, and have it parsed, by namespace etc");
        }

        public bool Equals(IMessagePointer messagePointer)
        {
            if (ReferenceEquals(null, messagePointer)) return false;
            if (ReferenceEquals(this, messagePointer)) return true;
            return Equals(MatchTag, messagePointer.MatchTag);
        }

        public override int GetHashCode()
        {
            return (MatchTag != null ? MatchTag.GetHashCode() : 0);
        }

        #region Implementation of ICanJson

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            TwitterMessagePointer messagePointer = new TwitterMessagePointer(long.Parse(ProviderMessageID));
            messagePointer.WriteJson(writer,serializer);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            throw new NotSupportedException();          
        }

        #endregion
    }
}