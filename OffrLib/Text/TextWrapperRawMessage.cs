using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Text
{
    public class TextWrapperRawMessage : RawMessage
    {
        public TextWrapperRawMessage(string sourceText)
        {
            Text = sourceText;
        }
    }
}
