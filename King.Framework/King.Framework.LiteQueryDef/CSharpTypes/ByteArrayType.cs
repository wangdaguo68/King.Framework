namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

    internal class ByteArrayType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            throw new ApplicationException("不支持格式ByteArray类型");
        }
    }
}
