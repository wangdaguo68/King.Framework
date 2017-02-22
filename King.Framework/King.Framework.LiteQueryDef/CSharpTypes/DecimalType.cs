namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

    internal class DecimalType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToDecimal(value).ToString();
        }
    }
}
