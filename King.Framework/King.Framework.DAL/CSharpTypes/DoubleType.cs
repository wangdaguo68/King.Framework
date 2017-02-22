namespace King.Framework.DAL.CSharpTypes
{
    using King.Framework.DAL;
    using System;
    using System.Text;

    internal class DoubleType : ISharpType
    {
        public object ConvertFrom(object value, Type targetType)
        {
            if (value == DBNull.Value)
            {
                return 0.0;
            }
            return Convert.ToDouble(value);
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
            return Convert.ToDouble(value).ToString();
        }

        public string GetOracleTypeString(Column col)
        {
            throw new NotImplementedException();
        }

        public string GetSqlTypeString(Column col)
        {
            if (col.IsPrimaryKey)
            {
                return "[float] NOT NULL";
            }
            return "[float] NULL";
        }

        public void SetValue(object obj, Column col, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                double naN = double.NaN;
                try
                {
                    naN = Convert.ToDouble(value);
                }
                catch
                {
                }
                col.PropertyInfo.SetValue(obj, naN, null);
            }
            else
            {
                col.PropertyInfo.SetValue(obj, 0.0, null);
            }
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
