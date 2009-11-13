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
    class TestModuleRegexTest : StandardModule
    {

        public override void Load()
        {
            //    IMessageParser
            //    ILocationProvider
            //    ISourceTextProvider
            //    IMessageProvider
            //    IMessageQueryProvider
            Bind<ITagProvider>().To<TagProvider>(); 
            Bind<IMessageParser>().To<RegexMessageParser>(); // <-- same as TestModule except for this
            Bind<IRawMessageProvider>().To<MockRawMessageProvider>();
            Bind<IMessageProvider>().To<MemoryMessageProvider>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
