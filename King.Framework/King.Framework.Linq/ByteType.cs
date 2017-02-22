namespace King.Framework.Linq
{
    using System;

    internal class ByteType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
