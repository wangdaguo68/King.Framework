namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class ShortType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if (value == DBNull.Value)
            {
                return (short)0;
            }
            return Convert.ToInt16(value);
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
            throw new NotImplementedException();
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "[smallint] NOT NULL";
            }
            return "[smallint] NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                value = Convert.ToInt16(value);
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
