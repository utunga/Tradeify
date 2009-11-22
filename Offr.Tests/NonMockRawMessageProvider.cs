using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Tests
{
    class NonMockRawMessageProvider : MockRawMessageProvider
    {
        public override void Update()
        {
            if (_updatedOnce) { return; }

            IList<IRawMessage> messages = new List<IRawMessage>();
            foreach (MockRawMessage rawMessage in MockData.RawMessages)
            {
                //string sourceText, IMessagePointer messagePointer, IUserPointer createdBy, string timestamp
                RawMessage raw = new RawMessage(rawMessage.Text, rawMessage.Pointer, rawMessage.CreatedBy, rawMessage.Timestamp);
                messages.Add(raw);
            }

            foreach (IRawMessageReceiver receiver in _receivers)
            {
                receiver.Notify(messages);
            }

            _updatedOnce = true;
        }
    }
    
}
