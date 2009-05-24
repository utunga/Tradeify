using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Core;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace Offr  
{
    public class DefaultNinjectConfig : StandardModule
    {
        public override void Load()
        {
            Bind<IMessageParser>().To<RegexMessageParser>(); //promoted regexparser to full value
            Bind<IRawMessageProvider>().To<Offr.Twitter.StatusProvider>();
            Bind<IMessageProvider>().To<MemoryMessageProvider>();
            Bind<IMessageQueryExecutor>().To<TagDexQueryExecutor>();
            Bind<ITagProvider>().To<TagProvider>();
            Bind<MessageProviderForKeywords>().ToSelf();
        }
    }
}
