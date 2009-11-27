using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Converters;
using Offr.Json.Converter;
using Offr.Message;

namespace Offr.Text
{
    public interface IRawMessage : IComparable<IRawMessage>,ICanJson
    {
        IMessagePointer Pointer { get; }
        IUserPointer CreatedBy { get; }
        string Text { get; }
        string Timestamp { get; }

    }
    //needed to avoid the json serializer throwing an error when trying to instantiate an IRawMessage interface!

}
