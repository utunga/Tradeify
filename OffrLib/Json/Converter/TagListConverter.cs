using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Message;
using Offr.Text;

namespace Offr.Json.Converter
{
    internal class TagListConverter : CanJsonConvertor<TagList>
    {
        public override TagList Create(JsonReader reader)
        {
            return new TagList();
        }
    }
}
