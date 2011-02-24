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

        private readonly IMessageRepository _messageRepository; 
        private readonly IMessageParser _messageParser;
        private readonly ITagRepository _tagRepository;
        private readonly IValidMessageReceiver _validMessageReceiver;

        public IncomingMessageProcessor(IMessageRepository messageRepository, ITagRepository tagRepository, IMessageParser messageParser, IValidMessageReceiver validMessageReceiver)
        {
            _messageRepository = messageRepository;
            _messageParser = messageParser;
            _tagRepository = tagRepository;
            _validMessageReceiver = validMessageReceiver;
        }

        public IncomingMessageProcessor(IMessageRepository messageRepository, ITagRepository tagRepository, IMessageParser messageParser) : this( messageRepository, tagRepository, messageParser, null)
        {
        }

        public IList<IMessage> AllMessages
        {
            get
            {
                return new List<IMessage>(_messageRepository.AllMessages());
            }
        }


        public void Invalidate()
        {
            _messageRepository.Invalidate();
        }

        public void Notify(IEnumerable<IRawMessage> updatedMessages)
        {
            List<IMessage> parsedMessages = new List<IMessage>();
            foreach (IRawMessage rawMessage in updatedMessages)
            {
                //bypass Retweets *hack FIXME
                if (rawMessage.Text.Trim().StartsWith("RT"))
                    continue;

                IMessage message = _messageParser.Parse(rawMessage);
                if (message.IsValid())
                {
                    parsedMessages.Add(message);
                }
                else
                {
                    if (message.ValidationFailReasons().Length <= 1)
                        _log.Info("Rejected almost valid message:" + Util.ConcatStringArray(message.ValidationFailReasons()) + " \"" + ((BaseMarketMessage)message).MessageText + "\"");
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
                    lock (_messageRepository)
                    {
                        _messageRepository.Save(parsedMessage);
                        //_tagRepository.s
                        foreach (ITag tag in parsedMessage.Tags)
                            _tagRepository.GetAndAddTagIfAbsent(tag.Text, tag.Type);
                    }

                    if (_validMessageReceiver!=null)
                    {
                        _validMessageReceiver.Push(parsedMessage);
                    }
                }
                else
                {
                    if (parsedMessage.ValidationFailReasons().Length <=1) 
                        _log.Info("Rejected almost valid message:" + Util.ConcatStringArray(parsedMessage.ValidationFailReasons()));
                }
            }
        }
    }
}
