using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Common;
using Offr.Message;
using Offr.Text;

namespace Offr.Query
{
    public class MessageReceivingTagDex : BaseTagDex, IMemCache, IMessageReceiver
    {
        private readonly IMessageProvider _messageProvider;

        public MessageReceivingTagDex(IMessageProvider messageProvider)
        {
            _messageProvider = messageProvider;
            _messageProvider.RegisterForUpdates(this);
            Invalidate(); // initialize with all the messages the messageProvider has got
        }

        protected override IEnumerable<IMessage> AllMessages()
        {
            return _messageProvider.AllMessages;
        }

        protected override void Update()
        {
            // this will end up calling back into the 'process' method here
            _messageProvider.Update();
        }

        public void Invalidate()
        {
            _seenTags = new List<ITag>();
            _index = new SortedList<string, List<IMessage>>();
           // _doubleTagIndex = new SortedList<string, List<IMessage>>();
            Process(_messageProvider.AllMessages);
        }

        public void Notify(IEnumerable<IMessage> updatedMessages)
        {
            Process(updatedMessages);
        }
    }
}
