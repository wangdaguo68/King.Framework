namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

    internal class ByteNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
