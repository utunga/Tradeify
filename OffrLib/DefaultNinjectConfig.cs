using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using Offr.Common;
using Offr.Location;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr  
{
    public class DefaultNinjectConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<IgnoredUserRepository>().ToSelf().InSingletonScope();
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IMessageRepository>().To<MessageRepository>().InSingletonScope();
            Bind<IRawMessageProvider>().To<TwitterRawMessageProvider>().InSingletonScope();
            Bind<IRawMessageReceiver>().To<IncomingMessageProcessor>().InSingletonScope();
            Bind<ILocationProvider>().To<GoogleLocationProvider>().InSingletonScope();
            //Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagRepository>().To<TagRepository>().InSingletonScope(); ;
            //Bind<WebRequestFactory.WebRequestMethod>().To<WebRequestFactory>();
        }
    }
}
