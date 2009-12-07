using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Repository;

namespace Offr.Text
{
   public class OpenSocialMessageProvider :IRawMessageProvider
    {
        public string ProviderNameSpace
        {
            get { return "Open Social"; }
        }

       public void ParseMessage(string RawMessageText, string userName, string thumbnail)
       {
           IRawMessageReceiver messageReceiver = Global.Kernel.Get<IRawMessageReceiver>();
           IRawMessage message = RawMessage.From(RawMessageText,"100",userName,thumbnail);
           List<IRawMessage> messages = new List<IRawMessage>();
           messages.Add(message);
           messageReceiver.Notify(messages);
       }
        public void RegisterForUpdates(IRawMessageReceiver receiver)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRawMessage> ForQueryText(string query)
        {
            throw new NotImplementedException();
        }

        public IRawMessage ByID(string providerMessageID)
        {
            throw new NotImplementedException();
        }
    }
}
