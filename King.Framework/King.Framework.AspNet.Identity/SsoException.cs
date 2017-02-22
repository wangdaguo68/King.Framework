namespace King.Framework.AspNet.Identity
{
    using System;
    using System.Runtime.InteropServices;

    public class SsoException : ApplicationException
    {
        public static readonly int ExpiredToken = -30;
        public static readonly int InvalidToken = -20;
        public static readonly int NullToken = -10;
        public static readonly int UserNotExists = -40;

        public SsoException(string message, int errorCode = -1) : base(message)
        {
            base.HResult = errorCode;
        }
    }
}

