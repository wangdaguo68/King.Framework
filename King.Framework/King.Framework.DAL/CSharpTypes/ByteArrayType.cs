namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class ByteArrayType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            return value;
        }

        public void EncodeInto(StringBuilder sb, object value)
        {
            if (value != null)
            {
                sb.Append(Convert.ToBase64String((byte[])value));
            }
        }

        public string FormatAsSql(object value)
        {
            throw new ApplicationException("不支持格式ByteArray类型");
        }

        public string GetOracleTypeString(Column col)
        {
            throw new NotImplementedException();
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "[image] NOT NULL";
            }
            return "[image] NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if (value == DBNull.Value)
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
