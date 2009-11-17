using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterMessagePointer :TwitterMessagePointerConverter, IMessagePointer, ICanJson 
    {
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }
        public string ProviderNameSpace { get { return "twitter"; } }
        public string ProviderMessageID { get; private set; }
        public TwitterMessagePointer()
        {
        }

        public TwitterMessagePointer(long status_id)
        {
            ProviderMessageID = status_id.ToString();
        }

        public bool Equals(TwitterMessagePointer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ProviderMessageID, ProviderMessageID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TwitterMessagePointer)) return false;
            return Equals((TwitterMessagePointer) obj);
        }

        public override int GetHashCode()
        {
            return (ProviderMessageID != null ? ProviderMessageID.GetHashCode() : 0);
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
           serializer.Serialize(writer,MatchTag);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            serializer.Deserialize(reader, typeof (string));
        }
    }
}
