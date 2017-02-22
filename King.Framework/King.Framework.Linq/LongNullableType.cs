namespace King.Framework.Linq
{
    using System;

    internal class LongNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt64(value).ToString();
        }
    }
}
