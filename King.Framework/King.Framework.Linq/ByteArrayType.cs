namespace King.Framework.Linq
{
    using System;

    internal class ByteArrayType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            throw new ApplicationException("不支持格式ByteArray类型");
        }
    }
}
