namespace King.Framework.Common
{
    using System;

    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {
        }

        public MessageException(string format, params object[] args) : base(string.Format(format, args))
        {
        }
    }
}
