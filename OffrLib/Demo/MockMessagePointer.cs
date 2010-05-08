using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public string ProviderNameSpace { get { return "mock"; }}
        
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

        #region Implementation of ICanJsonObject

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "type", this.GetType().Name);
            JSON.WriteProperty(serializer, writer, "provider_name_space", ProviderNameSpace);
            JSON.WriteProperty(serializer, writer, "message_id", ProviderMessageID);
        }

        public void ReadJson(JObject jObject, JsonSerializer serializer)
        {
            string providerNameSpace = JSON.ReadProperty<string>(jObject, "provider_name_space");
            Debug.Assert("mock".Equals(providerNameSpace), "Expect name space to be 'mock' for this type");
            this.ProviderMessageID = JSON.ReadProperty<string>(jObject, "message_id");
        }
        #endregion
    }
}