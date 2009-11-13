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
    class TestModuleMockFeed : StandardModule
    {

        public override void Load()
        {

            //use fake data as source
            Bind<IMessageParser>().To<MockMessageParser>();
            Bind<IRawMessageProvider>().To<MockRawMessageProvider>();

            // rest is common 
            Bind<IMessageProvider>().To<MemoryMessageProvider>().Using<SingletonBehavior>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagProvider>().To<TagProvider>().Using<SingletonBehavior>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
