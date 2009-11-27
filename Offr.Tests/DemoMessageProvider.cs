using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Text;

namespace Offr.Tests
{
    class DemoMessageProvider : MockRawMessageProvider
    {
        public override void Update()
        {
            if (_updatedOnce) { return; }

            IList<IRawMessage> messages = new List<IRawMessage>();
            foreach (RawMessage rawMessage in DemoData.RawMessages)
            {
                messages.Add(rawMessage);
            }

            foreach (IRawMessageReceiver receiver in _receivers)
            {
                receiver.Notify(messages);
            }

            _updatedOnce = true;
        }
    }
    
}
