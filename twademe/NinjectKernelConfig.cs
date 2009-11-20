using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Ninject.Core.Behavior;
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

            //use fake data
            //Bind<IMessageParser>().To<MockMessageParser>();
            //Bind<IRawMessageProvider>().To<MockRawMessageProvider>();

            // use live data
            Bind<IMessageParser>().To<RegexMessageParser>();
            Bind<IRawMessageProvider>().To<Offr.Twitter.TwitterRawMessageProvider>().Using<SingletonBehavior>();
            Bind<IMessageProvider>().To<MessageProvider>().Using<SingletonBehavior>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>().Using<SingletonBehavior>();
            Bind<ITagProvider>().To<TagProvider>().Using<SingletonBehavior>();
            Bind<MessageProviderForKeywords>().ToSelf();

        }
    }
}
