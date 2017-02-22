namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class DecimalNullableType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDecimal(value);
        }

        public object Decode(Type targetType, string value)
        {
            if (value == null)
            {
                return null;
            }
            return Convert.ToDecimal(value);
        }

        public string Encode(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }

        public void EncodeInto(StringBuilder sb, object value)
        {
            if (value != null)
            {
                sb.Append(value.ToString());
            }
        }

        public string FormatAsSql(object value)
        {
            return Convert.ToDecimal(value).ToString();
        }

        public string GetOracleTypeString(Column col)
        {
            throw new NotImplementedException();
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "[decimal](38,8) NOT NULL";
            }
            return "[decimal](38,8) NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                value = Convert.ToDecimal(value);
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
