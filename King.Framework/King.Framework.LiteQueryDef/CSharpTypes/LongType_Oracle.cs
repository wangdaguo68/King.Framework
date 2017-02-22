using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace King.Framework.LiteQueryDef.CSharpTypes
{
    internal class LongType_Oracle : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return Convert.ToInt64(value).ToString();
        }
    }
}
