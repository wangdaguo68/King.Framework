namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;


    internal class ByteType_Oracle : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
