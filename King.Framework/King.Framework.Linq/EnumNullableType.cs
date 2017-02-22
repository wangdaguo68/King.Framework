namespace King.Framework.Linq
{
    using System;

    internal class EnumNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
