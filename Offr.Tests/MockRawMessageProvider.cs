using System;
using System.Collections.Generic;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    public class MockRawMessageProvider : IRawMessageProvider
    {
        private bool _updatedOnce;
        private readonly List<IRawMessageReceiver> _receivers;

        public MockRawMessageProvider()
        {
            _updatedOnce=false;
             _receivers = new List<IRawMessageReceiver>();
        }

        public string ProviderNameSpace
        {
            get { return "Test"; }
        }

        public void RegisterForUpdates(IRawMessageReceiver receiver)
        {
            if (!_receivers.Contains(receiver))
            {
                _receivers.Add(receiver);
            }
        }

        public void Update()
        {
            if (_updatedOnce) { return; }
            
            IList<IRawMessage> messages = new List<IRawMessage>();
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                messages.Add(rawMessage);
            }

            foreach (IRawMessageReceiver receiver in _receivers)
            {
                receiver.Notify(messages);
            }
            
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