using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace King.Framework.LiteQueryDef.CSharpTypes
{

    internal class StringType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            string str = value.ToString().Replace("'", "''");
            return string.Format("'{0}'", str);
        }
    }
}
