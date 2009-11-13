using System;
using System.Runtime.Serialization;

[Serializable]
public class MessageParseException : ApplicationException
{

    public MessageParseException()
    {
    }

    public MessageParseException(string message) : base(message)
    {
    }

    public MessageParseException(string message, Exception inner) : base(message, inner)
    {
    }

    protected MessageParseException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}