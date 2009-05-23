using System;
using System.Collections.Generic;
using System.Linq;
using Offr.Text;

namespace Offr.Message
{
    public class MessageProviderForKeywords : IMessageProvider
    {
        private SortedList<string, IMessage> _messages;
        readonly IRawMessageProvider _sourceProvider;
        readonly IMessageParser _messageParser;
        private string _keywords;

        public MessageProviderForKeywords(IRawMessageProvider sourceProvider, IMessageParser messageParser, string keywords)
        {
            _sourceProvider = sourceProvider;
            _messageParser = messageParser;
            _keywords = keywords;
        }
       
        public IList<IMessage> AllMessages
        {
            get 
            {
                Update();
                return _messages.Values;
            }
        }

        public void Update()
        {
            _messages = new SortedList<string, IMessage>();
            foreach (IRawMessage rawMessage in _sourceProvider.ForQueryText(_keywords))
            {
                IMessage message = _messageParser.Parse(rawMessage);
                if (!message.IsValid) continue;
                _messages.Add(message.Source.Pointer.ProviderMessageID, message);
            }
        }

        public void RegisterForUpdates(IMessageReceiver receiver)
        {
            // not implemented
        }

        public IMessage MessageByID(string providerMessageID)
        {
            // currently assumes that we have only the one provider namespace - 
            if (_messages.ContainsKey(providerMessageID))
            {
                return _messages[providerMessageID];
            }
            return null;
        }

        public IMessage MessageByID(IMessagePointer messagePointer)
        {
            if (messagePointer.ProviderNameSpace != _sourceProvider.ProviderNameSpace)
            {
                throw new NotImplementedException(messagePointer.ProviderNameSpace + " not supported yet");
            }
            return MessageByID(messagePointer.ProviderMessageID);
        }
    }
}