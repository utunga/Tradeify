using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Message;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
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

        #region Implementation of ICanJson

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            //JSON.WriteProperty(serializer, writer, "ProviderNameSpace", ProviderNameSpace);
            //JSON.WriteProperty(serializer, writer, "MessageID", ProviderMessageID);
            //JSON.WriteProperty(serializer, writer, "Source", Source);
            TwitterMessagePointer messagePointer = new TwitterMessagePointer(long.Parse(ProviderMessageID));
            messagePointer.WriteJson(writer,serializer);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            JSON.ReadProperty<string>(serializer, reader, "ProviderNameSpace");
            this.ProviderMessageID = JSON.ReadProperty<string>(serializer, reader, "MessageID");
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(IMessagePointer)) return false;
            return Equals(((IMessagePointer)obj).MatchTag, MatchTag);
        }
        #endregion
    }
}
