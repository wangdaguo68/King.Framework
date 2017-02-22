using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace King.Framework.Linq.CSharpTypes
{
    internal interface ISharpType
    {
        string FormatAsSql(object value);
    }
}
