namespace King.Framework.Common
{
    using System;

    public class SetPropertyValueException : Exception
    {
        public SetPropertyValueException()
        {
        }

        public SetPropertyValueException(string msg) : base(msg)
        {
        }
    }
}
