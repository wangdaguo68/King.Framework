namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class GuidType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                return new Guid();
            }
            if (value.GetType() == typeof(Guid))
            {
                return value;
            }
            return new Guid(value.ToString());
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
            return string.Format("'{0}'", value);
        }

        public string GetOracleTypeString(Column col)
        {
            throw new NotImplementedException();
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "[uniqueidentifier] NOT NULL";
            }
            return "[uniqueidentifier] NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                value = new Guid(value.ToString());
            }
            else
            {
                value = new Guid();
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
