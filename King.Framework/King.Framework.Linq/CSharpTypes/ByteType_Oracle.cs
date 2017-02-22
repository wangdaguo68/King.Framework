namespace King.Framework.Linq.CSharpTypes
{
    using King.Framework.Linq;
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
