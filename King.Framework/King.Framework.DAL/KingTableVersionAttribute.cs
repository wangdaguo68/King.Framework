using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace King.Framework.DAL
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class KingTableVersionAttribute : Attribute
    {
    }
}
