using System;
using System.Collections.Generic;
using Ninject;
using Offr.Demo;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    public class MockRawMessageProvider : IRawMessageProvider
    {
        protected bool _updatedOnce;
        private readonly IRawMessageReceiver _receiver;

        [Inject]
        public MockRawMessageProvider(IRawMessageReceiver receiver)
        {
            _updatedOnce=false;
            _receiver = receiver;
        }

        public string ProviderNameSpace
        {
            get { return "Test"; }
        }

        public virtual void Update()
        {
            if (_updatedOnce) { return; }
            
            IList<IRawMessage> messages = new List<IRawMessage>();
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                messages.Add(rawMessage);
            }

            _receiver.Notify(messages);
            
            _updatedOnce = true;
        }

        public IEnumerable<IRawMessage> ForQueryText(string query)
        {
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                if (rawMessage.ToString().ToLower().Contains(query.ToLower()))
                {
                    yield return rawMessage;
                }
            }
        }

        public IRawMessage ByID(string providerMessageID)
        {
            if (providerMessageID == null) return null;
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                if (rawMessage.Pointer.ProviderMessageID.Equals(providerMessageID)) 
                {
                    return rawMessage;
                }
            }
            return null;
        }
    }
}