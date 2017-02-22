namespace King.Framework.Linq
{
    using System;

    internal class DecimalType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToDecimal(value).ToString();
        }
    }
}
