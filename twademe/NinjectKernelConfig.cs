using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Ninject.Core.Behavior;
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
            Bind<IMessageProvider>().To<IncomingMessageProcessor>().Using<SingletonBehavior>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagRepository>().To<TagRepository>().Using<SingletonBehavior>();
            
        }
    }
}
