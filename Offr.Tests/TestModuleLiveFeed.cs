using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;
using Ninject.Core.Behavior;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace Offr.Tests
{
    class TestModuleLiveFeed : StandardModule
    {

        public override void Load()
        {

            // use live data
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IRawMessageProvider>().To<Offr.Twitter.StatusProvider>().Using<SingletonBehavior>();
            Bind<IMessageProvider>().To<MemoryMessageProvider>().Using<SingletonBehavior>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagProvider>().To<TagProvider>().Using<SingletonBehavior>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
