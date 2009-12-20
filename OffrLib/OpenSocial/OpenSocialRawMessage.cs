using System;
using Offr.Message;
using Offr.Text;

namespace Offr.OpenSocial
{
    public class OpenSocialRawMessage : RawMessage, IRawMessage
    {

        public OpenSocialRawMessage(string nameSpace, string rawText, string id, string screenName, string thumbnail)
        {
            base.CreatedBy = new OpenSocialUserPointer(nameSpace, screenName, thumbnail);
            base.Pointer = new OpenSocialMessagePointer(nameSpace, id);
            base.Text = rawText;
            base.Timestamp = DateTime.Now.ToUniversalTime();
        }

    }
}