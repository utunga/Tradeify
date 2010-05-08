using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using Offr;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace twademe
{
    public class NinjectKernelConfig : NinjectModule
    {

        public override void Load()
        {
            // use live data
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IRawMessageProvider>().To<Offr.Twitter.TwitterRawMessageProvider>().InSingletonScope();
            Bind<IRawMessageReceiver>().To<IncomingMessageProcessor>().InSingletonScope();

            //ugly but it works - since messageRepository also implements 
            Bind<IMessageRepository>().To<MessageRepository>().InSingletonScope();
            Bind<ITagRepository>().To<TagRepository>().InSingletonScope();
            
        }
    }
}
