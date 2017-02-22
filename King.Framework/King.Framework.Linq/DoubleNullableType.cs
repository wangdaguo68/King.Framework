namespace King.Framework.Linq
{
    using System;

    internal class DoubleNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToDouble(value).ToString();
        }
    }
}
