using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Modules;
using Offr.Common;
using Offr.CouchDB;
using Offr.Location;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.RSS;
using Offr.Text;
using Offr.Twitter;

namespace Offr  
{
    public class DefaultNinjectConfig : NinjectModule
    {
        public override void Load()
        {
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IMessageRepository>().To<MessageRepository>().InSingletonScope();
            Bind<IRawMessageProvider>().To<TwitterRawMessageProvider>().InSingletonScope();
            Bind<IRawMessageReceiver>().To<IncomingMessageProcessor>().InSingletonScope();
            Bind<ILocationProvider>().To<GoogleLocationProvider>().InSingletonScope();
            //Bind<IgnoredUserRepository>().ToSelf().InSingletonScope();
            Bind<IUserPointerRepository>().To <IgnoredUserRepository>().InSingletonScope();

            Bind<IRSSRawMessageRepository>().To<RSSRawMessageRepository>().InSingletonScope();
            Bind<RSSRawMessageProvider>().ToSelf().InSingletonScope(); //seen updates
            //Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagRepository>().To<TagRepository>().InSingletonScope();
            Bind<IWebRequestFactory>().To<WebRequestFactory>();
            Bind<IValidMessageReceiver>().To<PushToCouchDBReceiver>();  // added for chchneeds
            
        }
    }
}
