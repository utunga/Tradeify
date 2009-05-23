using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace Offr.Tests
{
    class TestModule : StandardModule
    {

        public override void Load()
        {
            //    IMessageParser
            //    ILocationProvider
            //    ISourceTextProvider
            //    IMessageProvider
            //    IMessageQueryProvider
            Bind<IMessageParser>().To<MockMessageParser>();
            Bind<IRawMessageProvider>().To<MockRawMessageProvider>();
            Bind<IMessageProvider>().To<MemoryMessageProvider>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>();
            Bind<ITagProvider>().To<TagProvider>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
