using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Offr.Message;
using Offr.Query;
using Offr.Tests;
using Offr.Text;

namespace twademe
{
    public class NinjectKernelConfig :  StandardModule
    {

        public override void Load()
        {
            //    IMessageParser
            //    ILocationProvider
            //    ISourceTextProvider
            //    IMessageProvider
            //    IMessageQueryProvider

            //use fake data
            Bind<IMessageParser>().To<MockMessageParser>();
            Bind<IRawMessageProvider>().To<MockRawMessageProvider>();

            //Bind<IMessageParser>().To<DumbMessageParser>(); //NEEDS MES ONE OF THESE
            //Bind<IRawMessageProvider>().To<Offr.Twitter.StatusProvider>();
            Bind<IMessageProvider>().To<MemoryMessageProvider>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>();
            Bind<ITagProvider>().To<TagProvider>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
