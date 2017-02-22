namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class StringType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                return null;
            }
            return value.ToString();
        }

        public object Decode(Type targetType, string value)
        {
            return value;
        }

        public string Encode(object value)
        {
            return (string)value;
        }

        public void EncodeInto(StringBuilder sb, object value)
        {
            if (value != null)
            {
                TableSerializer.EncodeAndAppend(sb, (string)value);
            }
        }

        public string FormatAsSql(object value)
        {
            return value.ToString().Replace("'", "''");
        }

        public string GetOracleTypeString(Column col)
        {
            string str = !col.Length.HasValue ? "MAX" : col.Length.ToString();
            string str2 = col.IsPrimaryKey ? "NOT" : "";
            return string.Format("varchar2({0}) {1} NULL", str, str2);
        }

        public string GetSqlTypeString(Column col)
        {
            string str = !col.Length.HasValue ? "MAX" : col.Length.ToString();
            string str2 = col.IsPrimaryKey ? "NOT" : "";
            return string.Format("nvarchar({0}) {1} NULL", str, str2);
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value == null) || (value == DBNull.Value))
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
