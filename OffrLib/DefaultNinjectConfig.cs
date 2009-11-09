using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Ninject.Core.Behavior;
using Offr.Location;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace Offr  
{
    public class DefaultNinjectConfig : StandardModule
    {
        public override void Load()
        {
            //regexparser is *almost* good enough but not quite
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IRawMessageProvider>().To<Offr.Twitter.StatusProvider>().Using<SingletonBehavior>();
            Bind<IMessageProvider>().To<MemoryMessageProvider>().Using<SingletonBehavior>();
            Bind<ILocationProvider>().To<GoogleLocationProvider>().Using<SingletonBehavior>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagProvider>().To<TagProvider>().Using<SingletonBehavior>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
