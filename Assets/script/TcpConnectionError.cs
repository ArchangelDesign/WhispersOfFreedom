using System;
using System.Runtime.Serialization;

[Serializable]
internal class TcpConnectionError : Exception
{
    public TcpConnectionError()
    {
    }

    public TcpConnectionError(string message) : base(message)
    {
    }

    public TcpConnectionError(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TcpConnectionError(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}