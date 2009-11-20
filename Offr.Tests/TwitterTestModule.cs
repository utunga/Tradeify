using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Core;
using Offr.Message;
using Offr.Query;
using Offr.Text;
using Offr.Twitter;

namespace Offr.Tests
{
    class TwitterTestModule : StandardModule
    {

        public override void Load()
        {
            //    IMessageParser
            //    ILocationProvider
            //    ISourceTextProvider
            //    IMessageProvider
            //    IMessageQueryProvider
            Bind<IMessageParser>().To<MessageParser>(); 
            Bind<IRawMessageProvider>().To<Offr.Twitter.TwitterRawMessageProvider>();
            Bind<IMessageProvider>().To<MessageProvider>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>();
            Bind<ITagProvider>().To<TagProvider>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
