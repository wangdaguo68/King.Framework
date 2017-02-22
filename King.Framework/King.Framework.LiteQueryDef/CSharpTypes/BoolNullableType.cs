namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

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
