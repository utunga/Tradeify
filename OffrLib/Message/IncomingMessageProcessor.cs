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
        private readonly IAllMessageReceiver _allMessageReceiver;

        public IncomingMessageProcessor(IMessageRepository messageRepository, ITagRepository tagRepository, IMessageParser messageParser, IValidMessageReceiver validMessageReceiver, IAllMessageReceiver allMessageReceiver)
        {
            _messageRepository = messageRepository;
            _messageParser = messageParser;
            _tagRepository = tagRepository;
            _validMessageReceiver = validMessageReceiver;
            _allMessageReceiver = allMessageReceiver;
        }

        public IncomingMessageProcessor(IMessageRepository messageRepository, ITagRepository tagRepository, IMessageParser messageParser) : this( messageRepository, tagRepository, messageParser, null,null)
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
            int accepted = 0;
            int rejected = 0;
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
                    accepted++;
                }
                else
                {
                    if (message.ValidationFailReasons().Length <= 1)
                        _log.Info("Rejected almost valid message:" + Util.ConcatStringArray(message.ValidationFailReasons()) + " \"" + ((BaseMarketMessage)message).MessageText + "\"");
                    rejected++;
                }
            }
            Notify(parsedMessages);
            _log.Warn("Processed {0} messages. Rejected {1} as invalid, Accepted {2} into the db as valid.", (accepted+rejected), rejected, accepted);

        }

        public void Notify(IRawMessage updatedMessage)
        {
            // pass to above method
            Notify(new[] { updatedMessage });
        }

        public void Notify(IEnumerable<IMessage> parsedMessages) {

            foreach (IMessage parsedMessage in parsedMessages)
            {
                _allMessageReceiver.Push(parsedMessage);
                if (parsedMessage.IsValid())
                {
                    lock (_messageRepository)
                    {
                        if (_validMessageReceiver != null)
                        {
                            if (_messageRepository.Get(parsedMessage.ID) != null)
                                _log.Warn("Message already in local repository, won't push to valid CouchDB - '" + parsedMessage.RawText + "'");
                            else 
                                _validMessageReceiver.Push(parsedMessage);
                        }

                        _messageRepository.Save(parsedMessage);
                        //_tagRepository.s
                        foreach (ITag tag in parsedMessage.Tags)
                            _tagRepository.GetAndAddTagIfAbsent(tag.Text, tag.Type);
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
