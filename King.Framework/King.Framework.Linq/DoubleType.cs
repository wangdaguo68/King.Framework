namespace King.Framework.Linq
{
    using System;

    internal class DoubleType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToDouble(value).ToString();
        }
    }
}
