using Offr.Message;

namespace Offr.RSS
{
    public class RSSMessagePointer : MessagePointerBase
    {
        public sealed override string ProviderNameSpace { 
            get; 
            protected set;
        }

        internal RSSMessagePointer()
        {
        }

        public RSSMessagePointer(string sourceURI, string id)
        {
            ProviderNameSpace = sourceURI;
            ProviderMessageID = id;
        }
    }
}