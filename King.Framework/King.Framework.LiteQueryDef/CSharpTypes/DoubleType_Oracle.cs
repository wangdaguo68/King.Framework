namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

    internal class DoubleType_Oracle : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToDouble(value).ToString();
        }
    }
}
