namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

    internal class DecimalNullableType_Oracle : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToDecimal(value).ToString();
        }
    }
}
