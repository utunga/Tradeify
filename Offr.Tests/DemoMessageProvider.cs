using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;
using Offr.Demo;
using Offr.Text;

namespace Offr.Tests
{
    class DemoMessageProvider : IRawMessageProvider
    {
        readonly IRawMessageReceiver _receiver;
        private bool _updatedOnce;

        public string ProviderNameSpace
        {
            get { return DemoData.DemoNameSpace; }
        }

        [Inject]
        public DemoMessageProvider(IRawMessageReceiver receiver)
        {
            _updatedOnce = false;
            _receiver = receiver;
        }

        public void Update()
        {
            if (_updatedOnce) { return; }

            IList<IRawMessage> messages = new List<IRawMessage>();
            foreach (RawMessage rawMessage in DemoData.RawMessages)
            {
                messages.Add(rawMessage);
            }
   
            _receiver.Notify(messages);

            _updatedOnce = true;
        }
    }
    
}
