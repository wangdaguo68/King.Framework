﻿namespace King.Framework.LiteQueryDef.CSharpTypes
{
    using King.Framework.LiteQueryDef;
    using System;
    using System.Text;

    internal class DateTimeType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("'");
            builder.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append("'");
            return builder.ToString();
        }
    }
}
