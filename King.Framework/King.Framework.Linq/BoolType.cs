namespace King.Framework.Linq
{
    using System;

    internal class BoolType : ISharpType
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
