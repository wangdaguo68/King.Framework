﻿namespace King.Framework.Linq.CSharpTypes
{
    using King.Framework.Linq;
    using System;
    using System.Text;

    internal class DateTimeNullableType_Oracle : ISharpType
    {
        public string FormatAsSql(object value)
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("to_date('");
            builder.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append("','YYYY-MM-DD HH24:MI:SS')");
            return builder.ToString();
        }
    }
}
