namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class DateTimeNullableType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDateTime(value);
        }

        public void EncodeInto(StringBuilder sb, object value)
        {
            if (value != null)
            {
                sb.Append(Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        public string FormatAsSql(object value)
        {
            StringBuilder builder = new StringBuilder(0x20);
            builder.Append("'");
            builder.Append(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append("'");
            return builder.ToString();
        }

        public string GetOracleTypeString(Column col)
        {
            return "date";
        }

        public string GetSqlTypeString(Column col)
        {
            return "date";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                value = Convert.ToDateTime(value);
            }
            else
            {
                value = null;
            }
            col.PropertyInfo.SetValue(obj, value, null);
        }

        public bool AllowToBeKey
        {
            get
            {
                return false;
            }
        }
    }
}
