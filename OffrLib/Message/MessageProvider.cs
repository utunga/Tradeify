using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Common;
using Offr.Repository;
using Offr.Text;

namespace Offr.Message
{
    public class MessageProvider : IMessageProvider, IRawMessageReceiver, IMemCache
    {
        private readonly IMessageRepository _messages; 
        private readonly IRawMessageProvider _sourceProvider;
        private readonly IMessageParser _messageParser;
        private readonly List<IMessageReceiver> _receivers;

        public MessageProvider(IMessageRepository messageRepository, IRawMessageProvider sourceProvider, IMessageParser messageParser)
        {
            _messages = messageRepository;
            _sourceProvider = sourceProvider;
            _messageParser = messageParser;
            _receivers = new List<IMessageReceiver>();
            _sourceProvider.RegisterForUpdates(this);
            _sourceProvider.Update();
        }

        public IList<IMessage> AllMessages
        {
            get
            {
                return new List<IMessage>(_messages.AllMessages());
            }
        }

        public void Update()
        {
            // we expect that calling the below is likely to result in calls back to Notify() 
            _sourceProvider.Update();
        }

        public void RegisterForUpdates(IMessageReceiver receiver)
        {
            if (!_receivers.Contains(receiver))
            {
                _receivers.Add(receiver);
            }
        }

        public void Invalidate()
        {
            _messages.Invalidate();
            _sourceProvider.Update();
        }


        public void Notify(IEnumerable<IRawMessage> updatedMessages)
        {
            List<IMessage> parsedMessages = new List<IMessage>();
            foreach (IRawMessage rawMessage in updatedMessages)
            {
                IMessage message = _messageParser.Parse(rawMessage);
                if (message.IsValid)
                {
                    parsedMessages.Add(message);
                }
            }

            Notify(parsedMessages);
        }

        public void Notify(IEnumerable<IMessage> parsedMessages) {

            foreach (IMessage parsedMessage in parsedMessages)
            {
                if (parsedMessage.IsValid)
                {
                    InsertMessageToCache(parsedMessage.MessagePointer.ProviderMessageID, parsedMessage);
                }
            }

            List<IMessage> messages = new List<IMessage>(parsedMessages);

            // notify the older ones last
            messages.Sort();
            messages.Reverse();

            // pass on notification of updated messages
            if (messages.Count > 0)
            {
                foreach (IMessageReceiver receiver in _receivers)
                {
                    receiver.Notify(messages);
                }
            }
        }

        private void InsertMessageToCache(string providerMessageID, IMessage message)
        {
            lock (_messages)
            {
                _messages.Save(message);
            }
        }

       
    }

}
