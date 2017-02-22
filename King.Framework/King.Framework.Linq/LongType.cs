namespace King.Framework.Linq
{
    using System;

    internal class LongType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt64(value).ToString();
        }
    }
}
