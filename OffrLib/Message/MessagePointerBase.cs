using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Offr.Json;

namespace Offr.Message
{
    public abstract class MessagePointerBase : IMessagePointer
    {
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }

        public abstract string ProviderNameSpace { get; protected set; }

        public string ProviderMessageID { get; protected set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return ((obj is IMessagePointer) ? this.Equals((IMessagePointer)obj) : false);
        }

        public bool Equals(IMessagePointer messagePointer)
        {
            return Equals(MatchTag,messagePointer.MatchTag);
        }

        public override int GetHashCode()
        {
            return (MatchTag != null ? MatchTag.GetHashCode() : 0);
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer, writer, "type", this.GetType().Name);
            JSON.WriteProperty(serializer, writer, "provider_name_space", ProviderNameSpace);
            JSON.WriteProperty(serializer, writer, "message_id", ProviderMessageID);
        }

        public void ReadJson(JObject jObject, JsonSerializer serializer)
        {
            this.ProviderNameSpace = JSON.ReadProperty<string>(jObject, "provider_name_space");
            this.ProviderMessageID = JSON.ReadProperty<string>(jObject, "message_id");   
        }
    }
}