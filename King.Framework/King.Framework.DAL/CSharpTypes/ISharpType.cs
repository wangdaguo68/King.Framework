using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace King.Framework.DAL.CSharpTypes
{
    public interface ISharpType
    {
        object ConvertFrom(object value, Type targetType);
        void EncodeInto(StringBuilder sb, object value);
        string FormatAsSql(object value);
        string GetOracleTypeString(Column col);
        string GetSqlTypeString(Column col);
        void SetValue(object obj, Column col, object value);

        bool AllowToBeKey { get; }
    }
}
