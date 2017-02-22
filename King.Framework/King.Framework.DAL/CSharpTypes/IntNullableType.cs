namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class IntNullableType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            return Convert.ToInt32(value);
        }

        public object Decode(Type targetType, string value)
        {
            return Convert.ToInt32(value);
        }

        public string Encode(object value)
        {
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
            return Convert.ToInt32(value).ToString();
        }

        public string GetOracleTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "int NOT NULL";
            }
            return "int NULL";
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "int NOT NULL";
            }
            return "int NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                value = Convert.ToInt32(value);
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
                return true;
            }
        }
    }
}
