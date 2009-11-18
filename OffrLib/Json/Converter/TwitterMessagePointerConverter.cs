using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Offr.Twitter;

namespace Offr.Json.Converter
{
    public class TwitterMessagePointerConverter : CanJsonConvertor<TwitterMessagePointer>
    {

        public override TwitterMessagePointer Create()
        {
            return new TwitterMessagePointer();
        }
    }
}
