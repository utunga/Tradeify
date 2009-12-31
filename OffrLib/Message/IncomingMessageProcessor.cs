using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Offr.Common;
using Offr.Repository;
using Offr.Text;

namespace Offr.Message
{
    public class IncomingMessageProcessor : IRawMessageReceiver, IMemCache
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly IMessageRepository _messages; 
        private readonly IMessageParser _messageParser;

        public IncomingMessageProcessor(IMessageRepository messageRepository, IMessageParser messageParser)
        {
            _messages = messageRepository;
            _messageParser = messageParser;
            //_sourceProvider.Update();
        }

        public IList<IMessage> AllMessages
        {
            get
            {
                return new List<IMessage>(_messages.AllMessages());
            }
        }


        public void Invalidate()
        {
            _messages.Invalidate();
        }

        public void Notify(IEnumerable<IRawMessage> updatedMessages)
        {
            List<IMessage> parsedMessages = new List<IMessage>();
            foreach (IRawMessage rawMessage in updatedMessages)
            {
                IMessage message = _messageParser.Parse(rawMessage);
                if (message.IsValid())
                {
                    parsedMessages.Add(message);
                }
                else
                {
                    _log.Info("Rejected invalid message:" + message);
                }
            }
            Notify(parsedMessages);
        }

        public void Notify(IRawMessage updatedMessage)
        {
            // pass to above method
            Notify(new[] { updatedMessage });
        }

        public void Notify(IEnumerable<IMessage> parsedMessages) {

            foreach (IMessage parsedMessage in parsedMessages)
            {
                if (parsedMessage.IsValid())
                {
                    lock (_messages)
                    {
                        _messages.Save(parsedMessage);
                    }
                }
            }
        }       
    }

}
