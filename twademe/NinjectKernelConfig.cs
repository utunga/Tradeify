using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Ninject.Core.Behavior;
using Offr;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public class NinjectKernelConfig :  StandardModule
    {

        public override void Load()
        {
            // use live data
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IRawMessageProvider>().To<Offr.Twitter.TwitterRawMessageProvider>().Using<SingletonBehavior>();
            Bind<IRawMessageReceiver>().To<IncomingMessageProcessor>().Using<SingletonBehavior>();

            //ugly but it works - since messageRepository also implements 
            Bind<IMessageRepository>().To<MessageRepository>().Using<SingletonBehavior>();
            Bind<ITagRepository>().To<TagRepository>().Using<SingletonBehavior>();
            
        }
    }
}
