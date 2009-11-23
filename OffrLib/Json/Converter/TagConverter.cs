using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Offr.Text;

namespace Offr.Json.Converter
{
    public class TagConverter : CanJsonConvertor<ITag>
    {
        public override ITag Create(JsonReader reader)
        {
            return new Tag();
        }
    }
}
