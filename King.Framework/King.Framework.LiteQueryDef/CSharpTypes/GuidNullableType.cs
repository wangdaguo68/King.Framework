using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace King.Framework.LiteQueryDef.CSharpTypes
{
    internal class GuidNullableType : ISharpType
    {
        public string FormatAsSql(object value)
        {
            return string.Format("'{0}'", value);
        }
    }
}
