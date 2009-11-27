using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Ninject.Core.Behavior;
using Offr.Location;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;
using Offr.Twitter;

namespace Offr  
{
    public class DefaultNinjectConfig : StandardModule
    {
        public override void Load()
        {
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IMessageRepository>().To<MessageRepository>().Using<SingletonBehavior>();
            Bind<IRawMessageProvider>().To<TwitterRawMessageProvider>().Using<SingletonBehavior>();
            Bind<IMessageProvider>().To<MessageProvider>().Using<SingletonBehavior>();
            Bind<ILocationProvider>().To<GoogleLocationProvider>().Using<SingletonBehavior>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagRepository>().To<TagRepository>().Using<SingletonBehavior>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
