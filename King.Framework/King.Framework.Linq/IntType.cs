namespace King.Framework.Linq
{
    using System;

    internal class IntType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
