using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Message;
using Offr.Text;

namespace Offr.Tests
{
    public class MockMessagePointer : IMessagePointer
    {
        public string MatchTag
        {
            get { return ProviderNameSpace + "/" + ProviderMessageID; }
        }
        public string SourceURL { get; private set; }
        public IResolvedURI ResolvedURL { get; private set; }
        public string ProviderMessageID { get; private set; }
        public string ProviderNameSpace { get { return "test"; }}
        
        public MockMessagePointer(int id)
        {
            ProviderMessageID = id.ToString();
        }

        public MockMessagePointer(string url)
        {
            throw new NotImplementedException("Ideally you can add a message pointer by url, and have it parsed, by namespace etc");
        }
    }
}
