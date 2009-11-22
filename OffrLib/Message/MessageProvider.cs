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
        private MessageRepository _messages; 
        private readonly IRawMessageProvider _sourceProvider;
        private readonly IMessageParser _messageParser;
        private readonly List<IMessageReceiver> _receivers;
      
        public MessageProvider(IRawMessageProvider sourceProvider, IMessageParser messageParser)
        {
            _sourceProvider = sourceProvider;
            _messageParser = messageParser;
            _receivers = new List<IMessageReceiver>();
            _sourceProvider.RegisterForUpdates(this);

            Invalidate();
        }

        public IList<IMessage> AllMessages
        {
            get
            {
                return new List<IMessage>(_messages.GetAll());
            }
        }

        public void Update()
        {
            UpdateMessageCache();
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
            _messages=new MessageRepository();
            _sourceProvider.Update();
        }

        public IMessage MessageByID(string providerMessageID)
        {
            // currently assumes that we have only the one provider namespace - namely the one of the _sourceProvider (ie probably twitter)
            
            // actually, if we skip this, we'll only return messages that have been parsed already, which
            // is kinda preferable given search api vs xml api.
            //UpdateMessageCacheForSingleItem(providerMessageID);

            //can be null
            return _messages.Get(providerMessageID); 
        }

        public IMessage MessageByID(IMessagePointer messagePointer)
        {
            // assume for now that we have only the one provider namespace - 
            // namely the one of the _sourceProvider (ie probably twitter)
            // and throw an exception otherwise
            // if messagePointer hasn't worked out its ProviderMessageID yet then returns nothing
            if (messagePointer.ProviderNameSpace != _sourceProvider.ProviderNameSpace)
            {
                throw new NotImplementedException(messagePointer.ProviderNameSpace + " not supported yet");
            }
            return MessageByID(messagePointer.ProviderMessageID);
        }

        //private IMessage UpdateMessageCacheForSingleItem(string providerMessageID)
        //{
        //    IRawMessage rawMessage = _sourceProvider.ByID(providerMessageID);
        //    if (rawMessage != null)
        //    {
        //        IMessage message = _messageParser.Parse(rawMessage);
        //        if (message.IsValid)
        //        {
        //            InsertMessageToCache(rawMessage.Pointer.ProviderMessageID, message);
        //            return message;
        //        }
        //    }
        //    return null;
        //}

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
                    InsertMessageToCache(parsedMessage.Source.Pointer.ProviderMessageID, parsedMessage);
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

        public void InitializeFromFile(string filePath)
        {
            Invalidate();
            _messages.InitializeFromFile(filePath);
        }

        private void UpdateMessageCache()
        {
            // we expect that calling the below is likely to cause, calls back to Notify() above
            _sourceProvider.Update();
        }

        private void InsertMessageToCache(string providerMessageID, IMessage message)
        {
            lock (_messages)
            {
                _messages.Save(message);
/*                if (_messages.ContainsKey(providerMessageID))
                {
                    _messages[providerMessageID] = message;
                }
                else
                {
                    _messages.Add(providerMessageID, message);
                } */
            }
        }

       
    }

}
