﻿namespace King.Framework.Linq.CSharpTypes
{
    using King.Framework.Linq;
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
