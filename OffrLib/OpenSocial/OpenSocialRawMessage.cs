using System;
using Offr.Message;
using Offr.Text;

namespace Offr.OpenSocial
{
    public class OpenSocialRawMessage : RawMessage, IRawMessage
    {

        public OpenSocialRawMessage(string nameSpace, string rawText, string screenName, string thumbnail, string profileUrl)
        {
            base.CreatedBy = new OpenSocialUserPointer(nameSpace, screenName, thumbnail, profileUrl);
            base.Pointer = new OpenSocialMessagePointer(nameSpace);
            base.Text = rawText;
            base.Timestamp = DateTime.Now.ToUniversalTime();
        }

    }
}