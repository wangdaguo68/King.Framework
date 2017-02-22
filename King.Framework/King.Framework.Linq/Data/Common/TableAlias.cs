namespace King.Framework.Linq.Data.Common
{
    using System;

    public class TableAlias
    {
        public override string ToString()
        {
            return ("A:" + this.GetHashCode());
        }
    }
}
