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


        public IncomingMessageProcessor(IMessageRepository messageRepository, ITagRepository tagRepository, IMessageParser messageParser)
        {
            _messageRepository = messageRepository;
            _messageParser = messageParser;
            _tagRepository = tagRepository;
            //_sourceProvider.Update();
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
                IMessage message = _messageParser.Parse(rawMessage);
                if (message.IsValid())
                {
                    parsedMessages.Add(message);
                }
                else
                {
                    _log.Info("Rejected invalid message:" + Util.ConcatStringArray(message.ValidationFailReasons()) + " \"" + message + "\"");
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
                }
                else
                {
                    _log.Info("Rejected invalid message:" + Util.ConcatStringArray(parsedMessage.ValidationFailReasons()));
                }
            }
        }
    }

}
