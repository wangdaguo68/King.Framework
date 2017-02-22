namespace King.Framework.Linq
{
    using System;

    internal class BoolNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            if (Convert.ToBoolean(value))
            {
                return "1";
            }
            return "0";
        }
    }
}
