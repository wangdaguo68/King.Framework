namespace King.Framework.Interfaces
{
    using System;

    public class ApplicationExceptionFormat : ApplicationException
    {
        public ApplicationExceptionFormat(string format, params object[] args) : base(string.Format(format, args))
        {
        }
    }
}

