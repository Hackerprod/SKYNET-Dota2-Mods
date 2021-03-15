using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

[Serializable]
internal sealed class BerDecodeException : Exception
{
    private readonly int _position;

    public override string Message
    {
        get
        {
            StringBuilder stringBuilder = new StringBuilder(base.Message);
            stringBuilder.AppendFormat(" (Position {0}){1}", _position, Environment.NewLine);
            return stringBuilder.ToString();
        }
    }

    public BerDecodeException(string message, int position)
        : base(message)
    {
        _position = position;
    }

    public BerDecodeException(string message, int position, Exception ex)
        : base(message, ex)
    {
        _position = position;
    }

    private BerDecodeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _position = info.GetInt32("Position");
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Position", _position);
    }
}