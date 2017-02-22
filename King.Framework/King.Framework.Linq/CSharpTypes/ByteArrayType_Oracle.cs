namespace King.Framework.Linq.CSharpTypes
{
    using King.Framework.Linq;
    using System;
    using System.Text;

    internal class ByteArrayType_Oracle : ISharpType
    {
        public string FormatAsSql(object value)
        {
            throw new ApplicationException("不支持格式ByteArray类型");
        }
    }
}
