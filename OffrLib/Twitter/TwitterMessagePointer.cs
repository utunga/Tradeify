using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Offr.Json;
using Offr.Json.Converter;
using Offr.Message;
using Offr.Text;

namespace Offr.Twitter
{
    public class TwitterMessagePointer :IMessagePointer 
    {
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }
        public string ProviderNameSpace { get { return "twitter"; } }
        public string ProviderMessageID { get; private set; }

        //FIXME1 remove this as its just a test
        public TwitterMessagePointer Source { get; set; }
        
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

        //writer.WritePropertyName("ProviderNameSpace");
        //writer.WriteValue(ProviderMessageID);
        //JSONConverterWrapper.WriteProperty("ProviderNameSpace", ProviderNameSpace);
        //writer.WritePropertyName("Source");

        //serializer.Serialize(writer, Source);
        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            JSON.WriteProperty(serializer,writer,"ProviderNameSpace", ProviderNameSpace);
            JSON.WriteProperty(serializer,writer, "MessageID", ProviderMessageID);
            JSON.WriteProperty(serializer,writer, "Source", Source);
        }

        public void ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            //JSONConverterWrapper.ReadAndAssertProperty(reader, "ProviderNameSpace");
            //JSONConverterWrapper.ReadAndAssertStringValue(reader, "twitter");
            JSON.ReadProperty<string>(serializer, reader, "ProviderNameSpace");
            this.ProviderMessageID = JSON.ReadProperty<string>(serializer, reader, "MessageID");//ReadStringProperty(reader, "MessageID");

            /*ReadAndAssertProperty(reader, "Source");
            ReadAndAssert(reader); //to get the outer 'tags' off
            this.Source = serializer.Deserialize(reader, GetType()) as TwitterMessagePointer;
            */
            this.Source = JSON.ReadProperty<TwitterMessagePointer>(serializer, reader, "Source");
            
        }

/*        private static string ReadStringProperty(JsonReader reader, string propertyName)
        {
            ReadAndAssertProperty(reader, propertyName);
            ReadAndAssert(reader);
            return reader.Value==null ? null : reader.Value.ToString();
        }*/


        //these methods might be good to move to a utils class or to a base class

        

    }
}
