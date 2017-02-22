namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class EnumNullableType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                Type enumType = targetType.GetGenericArguments()[0];
                return Enum.ToObject(enumType, Convert.ToInt32(value));
            }
            return null;
        }

        public object Decode(Type targetType, string value)
        {
            int num = Convert.ToInt32(value);
            return Enum.ToObject(targetType, num);
        }

        public string Encode(object value)
        {
            return Convert.ToInt32(value).ToString();
        }

        public void EncodeInto(StringBuilder sb, object value)
        {
            if (value != null)
            {
                sb.Append(Convert.ToInt32(value).ToString());
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
                Type enumType = col.DataType.GetGenericArguments()[0];
                value = Enum.ToObject(enumType, value);
            }
            else
            {
                value = SharpTypeHelper.GetDefaultValue(col.DataType);
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
