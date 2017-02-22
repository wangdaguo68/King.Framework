namespace King.Framework.Linq
{
    using System;
    using System.Text;

    internal class DateTimeNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("'");
            builder.Append(((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append("'");
            return builder.ToString();
        }
    }
}
