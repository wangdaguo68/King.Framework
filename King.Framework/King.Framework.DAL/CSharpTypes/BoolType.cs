namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class BoolType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                return false;
            }
            string str = value.ToString().ToUpper();
            return ((str == "TRUE") || (str == "1"));
        }

        public void EncodeInto(StringBuilder sb, object value)
        {
            if (value != null)
            {
                bool flag = Convert.ToBoolean(value);
                sb.Append(flag ? '1' : '0');
            }
        }

        public string FormatAsSql(object value)
        {
            if (Convert.ToBoolean(value))
            {
                return "1";
            }
            return "0";
        }

        public string GetOracleTypeString(Column col)
        {
            throw new NotImplementedException();
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "[bit] NOT NULL";
            }
            return "[bit] NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                value = Convert.ToBoolean(value);
            }
            else
            {
                value = false;
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
